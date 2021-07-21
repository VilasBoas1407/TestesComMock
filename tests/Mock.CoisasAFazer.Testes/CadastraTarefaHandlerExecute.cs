using System;
using Xunit;
using Alura.CoisasAFazer.Core.Commands;
using Alura.CoisasAFazer.Core.Models;
using Alura.CoisasAFazer.Services.Handlers;
using Alura.CoisasAFazer.Testes;
using System.Linq;
using Alura.CoisasAFazer.Infrastructure;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using Moq;
using Microsoft.Extensions.Logging;

namespace Mock.CoisasAFazer.Testes
{
    public class CadastraTarefaHandlerExecute
    {
        [Fact]
        public void DadaTarefaComInfoValidasDeveIncluirNoDB()
        {
            //Arrange
            var comando = new CadastraTarefa("Estudar XUnit", new Categoria("Estudo"), new DateTime(2021, 04, 28));

            var mock = new Mock<ILogger<CadastraTarefaHandler>>();
            var options = new DbContextOptionsBuilder<DbTarefasContext>().UseInMemoryDatabase("DbTarefasContext").Options;
            var context = new DbTarefasContext(options);
            var repo = new RepositorioTarefa(context);
            var handler = new CadastraTarefaHandler(repo, mock.Object);

            //Act
            handler.Execute(comando);


            //Assert
            var tarefas = repo.ObtemTarefas(t => t.Titulo == "Estudar XUnit").FirstOrDefault();
            Assert.NotNull(tarefas);
        }

        [Fact]
        public void DadaTarefaComInfoFaltando()
        {
            //Arrange
            var comando = new CadastraTarefa("Estudar XUnit 123456", new Categoria(""), new DateTime(2021, 04, 28));
            var mocklogger = new Mock<ILogger<CadastraTarefaHandler>>();
            var options = new DbContextOptionsBuilder<DbTarefasContext>().UseInMemoryDatabase("DbTarefasContext").Options;

            var contexto = new DbTarefasContext(options);
            var repo = new RepositorioTarefa(contexto);
            var handler = new CadastraTarefaHandler(repo, mocklogger.Object);

            //Act
            handler.Execute(comando);


            //Assert
            var tarefas = repo.ObtemTarefas(t => t.Titulo == "Estudar XUnit").FirstOrDefault();
            Assert.Null(tarefas);
        }

        [Fact]
        public void QuandoExceptioForLancadaResultadoIsSucessDeveSerFalse()
        {
            //Arrange
            var comando = new CadastraTarefa("Estudar XUnit", new Categoria("Estudo"), new DateTime(2021, 04, 28));
            var mocklogger = new Mock<ILogger<CadastraTarefaHandler>>();
            var mock = new Mock<IRepositorioTarefas>();
            mock.Setup(r => r.IncluirTarefas(It.IsAny<Tarefa[]>()))
                .Throws(new Exception("Houve um erro na inclusão de tarefas"));
            var repo = mock.Object;

            var handler = new CadastraTarefaHandler(repo, mocklogger.Object);

            //Act
            CommandResult resultado = handler.Execute(comando);

            //Assert
            Assert.False(resultado.IsSucess);
        }

        [Fact]
        public void QuandoExceptionForLancadaDeveLogarAMensagemDaExcecao()
        {
            //Arrange
            string mensagemDeErroEsperada = "Houve um erro na inclusão de tarefas";
            var comando = new CadastraTarefa("Estudar XUnit", new Categoria("Estudo"), new DateTime(2021, 04, 28));

            var mocklogger = new Mock<ILogger<CadastraTarefaHandler>>();
            var mock = new Mock<IRepositorioTarefas>();

            mock.Setup(r => r.IncluirTarefas(It.IsAny<Tarefa[]>()))
                .Throws(new Exception("Houve um erro na inclusão de tarefas"));
            var repo = mock.Object;

            var handler = new CadastraTarefaHandler(repo,mocklogger.Object);

            //Act
            CommandResult resultado = handler.Execute(comando);

            //Assert
            mocklogger.Verify(l => l.LogError(mensagemDeErroEsperada), Times.Once());

        }
    }
}
