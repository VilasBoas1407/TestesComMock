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

namespace Mock.CoisasAFazer.Testes
{
    public class CadastraTarefaHandlerExecute
    {
        [Fact]
        public void DadaTarefaComInfoValidasDeveIncluirNoDB()
        {
            //Arrange
            var comando = new CadastraTarefa("Estudar XUnit", new Categoria("Estudo"), new DateTime(2021, 04, 28));

            var repo = new RepositorioFake();
            var handler = new CadastraTarefaHandler(repo);

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

            var options = new DbContextOptionsBuilder<DbTarefasContext>().UseInMemoryDatabase("DbTarefasContext").Options;

            var contexto = new DbTarefasContext(options);
            var repo = new RepositorioTarefa(contexto);
            var handler = new CadastraTarefaHandler(repo);

            //Act
            handler.Execute(comando);


            //Assert
            var tarefas = repo.ObtemTarefas(t => t.Titulo == "Estudar XUnit").FirstOrDefault();
            Assert.Null(tarefas);
        }
    }
}
