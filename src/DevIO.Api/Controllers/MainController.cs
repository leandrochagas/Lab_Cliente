using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevIO.Business.Intefaces;
using DevIO.Business.Notificacoes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace DevIO.Api.Controllers
{
    public class MainController : Controller
    {
        private readonly INotificador _notificador;

        public MainController(INotificador notificador)
        {
            _notificador = notificador;
        }


        protected bool OperacaoValida()
        {
            return !_notificador.TemNotificacao();
        }

        protected ActionResult CustomResponse(object result = null)
        {
            if (OperacaoValida())
            {
                return Ok(new { sucess = true, data = result });
            }

            return BadRequest(new { sucess = false, erros = _notificador.ObterNotificacoes().Select(n => n.Mensagem) });
        }


        protected ActionResult CustomResponse(ModelStateDictionary modelstate)
        {
            if (!modelstate.IsValid) NotificarErroModelInvalida(modelstate);
            return CustomResponse();
        }


        protected void NotificarErroModelInvalida(ModelStateDictionary modelState)
        {
            var erros = modelState.Values.SelectMany(e => e.Errors);
            foreach (var erro in erros)
            {
                var errorMsg = erro.Exception == null ? erro.ErrorMessage : erro.Exception.Message;
                NotificarErro(errorMsg);
            }
        }
        protected void NotificarErro(string mensage)
        {
            _notificador.Handle(new Notificacao(mensage));
        }

    }
}