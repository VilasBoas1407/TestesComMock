﻿using Alura.CoisasAFazer.Core.Commands;
using Alura.CoisasAFazer.Core.Models;
using Alura.CoisasAFazer.Infrastructure;
using Alura.CoisasAFazer.Services.Handlers;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Alura.CoisasAFazer.Testes
{
    public class GerenciaPrazoDasTarefasHandlerExecute
    {
        [Fact]
        public void QuandoTarefasEstiveremAtrasadasDeveMudarSeuStatus()
        {
            //arrange
            var compCateg = new Categoria(1, "Compras");
            var casasCateg = new Categoria(2, "Casa");
            var trabCateg = new Categoria(3, "Trabalho");
            var saudCateg = new Categoria(4, "Saúde");
            var higiCateg = new Categoria(5, "Higiene");

            var tarefas = new List<Tarefa>
            {

                //atrasadas a partir de 1/1/2019
                new Tarefa(1,"Tirar o lixo", casasCateg, new DateTime(2018,12,31), null, StatusTarefa.Criada),
                new Tarefa(4,"Fazer almoço", casasCateg, new DateTime(2017,12,1),null,StatusTarefa.Criada),
                new Tarefa(9, "Ir á academia", saudCateg, new DateTime(2018,12,31), null, StatusTarefa.Criada),
                new Tarefa(7 , "Concluir o relatório", trabCateg, new DateTime(2018,5,7), null, StatusTarefa.Pendente),
                new Tarefa(10, "Beber água", saudCateg, new DateTime(2018,12,31), null, StatusTarefa.Concluida),
                
                //dentro do prazo 1/1/2019
                new Tarefa(8, "Comparecer á reunião", trabCateg, new DateTime(2018,12,12),new DateTime(2018,11,30),StatusTarefa.Concluida),
                new Tarefa(2, "Arrumar a cama", casasCateg,new DateTime(2019,1,2), null, StatusTarefa.Criada),
                new Tarefa(5, "Comprar presente pro João", higiCateg, new DateTime(2019,1,2), null, StatusTarefa.Criada),
                new Tarefa(6, "Comprar ração", compCateg, new DateTime(2019,11,20), null, StatusTarefa.Criada)
            };


            var options = new DbContextOptionsBuilder<DbTarefasContext>().UseInMemoryDatabase("DbTarefasContext").Options;
            var contexto = new DbTarefasContext(options);
            var repo = new RepositorioTarefa(contexto);

            repo.IncluirTarefas(tarefas.ToArray());

            var comando = new GerenciaPrazoDasTarefas(new DateTime(2019, 1, 1));
            var handler = new GerenciaPrazoDasTarefasHandler(repo);

            //act
            handler.Execute(comando);

            //assert
            var tarefasEmAtraso = repo.ObtemTarefas(t => t.Status == StatusTarefa.EmAtraso);
            Assert.Equal(4, tarefasEmAtraso.Count());
        }

        [Fact]
        public void QuandoInvocadoDeveChamarAtualizarTarefasQuantidadeDeVezesDoTotalDeTarefasAtrasadas()
        {
            //arrange
            var categ = new Categoria("Dummy");
            var mock = new Mock<IRepositorioTarefas>();

            var tarefas = new List<Tarefa>{
                new Tarefa(1,"Tirar o lixo", categ, new DateTime(2018,12,31), null, StatusTarefa.Criada),
                new Tarefa(4,"Fazer almoço", categ, new DateTime(2017,12,1),null,StatusTarefa.Criada),
                new Tarefa(9, "Ir á academia", categ, new DateTime(2018,12,31), null, StatusTarefa.Criada),
            };

            mock.Setup(r => r.ObtemTarefas(It.IsAny<Func<Tarefa, bool>>()))
                .Returns(tarefas);
            var repo = mock.Object;

            var comando = new GerenciaPrazoDasTarefas(new DateTime(2019, 1, 1));
            var handler = new GerenciaPrazoDasTarefasHandler(repo);

            //act
            handler.Execute(comando);
            
            //assert
            mock.Verify(r => r.AtualizarTarefas(It.IsAny<Tarefa[]>()), Times.Once());
        }
    }
}
