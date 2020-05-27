using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DevIO.Api.ViewModels;
using DevIO.Business.Intefaces;
using DevIO.Business.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevIO.Api.Controllers
{
    [ApiController]   
    [Route("api/v{version:apiVersion}/clientes")]
    public class ClientesController : MainController
    {
        private readonly IClienteRepository _clienteRepository;
        private readonly IMapper _mapper;
        private readonly IClienteService _clienteService;
        private readonly IEnderecoRepository _enderecoRepository;

        public ClientesController(IClienteRepository clienteRepository,
                                  IMapper mapper,
                                  IClienteService clienteService,
                                  INotificador notificador,
                                  IEnderecoRepository enderecoRepository) : base(notificador)
        {
            _clienteRepository = clienteRepository;
            _mapper = mapper;
            _clienteService = clienteService;
            _enderecoRepository = enderecoRepository;
        }

        #region Métodos Cliente
        [HttpGet("listar-todos")]
        public async Task<IEnumerable<ClienteViewModel>> ObterTodos()
        {
            var cliente = _mapper.Map<IEnumerable<ClienteViewModel>>(await _clienteRepository.ObterTodos());
            return cliente;
        }

        [HttpGet("obter-cliente-por-id/{id:guid}")]
        public async Task<ActionResult<ClienteViewModel>> ObterPorId(Guid id)
        {
            var cliente = _mapper.Map<ClienteViewModel>(await _clienteRepository.ObterClienteEndereco(id));
            if (cliente == null) return NotFound();

            return cliente;
        }

        [HttpGet("obter-endereco-cliente/{id:guid}")]
        public async Task<ActionResult<EnderecoViewModel>> ObterEnderecoPorId(Guid id)
        {
            return _mapper.Map<EnderecoViewModel>(await _enderecoRepository.ObterPorId(id));
        }

        [HttpPut("atualizar-cliente/{id:guid}")]
        public async Task<ActionResult<ClienteViewModel>> Atualizar(Guid id, ClienteViewModel clienteViewModel)
        {
            if (id != clienteViewModel.Id)
            {
                NotificarErro("O id informado não é o mesmo");
                return CustomResponse(clienteViewModel);
            }
            if (!ModelState.IsValid) return CustomResponse(clienteViewModel);

            await _clienteService.Atualizar(_mapper.Map<Cliente>(clienteViewModel));

            return CustomResponse(clienteViewModel);

        }

        [HttpPut("atualizar-endereco-cliente/{id:guid}")]
        //[HttpPut("atualizar-endereco/{id:guid}")]
        public async Task<IActionResult> AtualizarEndereco(Guid id, EnderecoViewModel enderecoViewModel)
        {
            if (id != enderecoViewModel.Id)
            {
                NotificarErro("O id informado não é o mesmo que foi passado na query");
                return CustomResponse(enderecoViewModel);
            }

            if (!ModelState.IsValid) return CustomResponse(ModelState);

            await _clienteService.AtualizarEndereco(_mapper.Map<Endereco>(enderecoViewModel));

            return CustomResponse(enderecoViewModel);
        }

        [HttpPost("salvar-cliente")]
        public async Task<ActionResult<ClienteViewModel>> Adicionar(ClienteViewModel clienteViewModel)
        {
            if (!ModelState.IsValid) return CustomResponse(ModelState);

            await _clienteService.Adicionar(_mapper.Map<Cliente>(clienteViewModel));

            return CustomResponse(clienteViewModel);

        }

        [HttpDelete("remover-cliente/{id:guid}")]
        public async Task<ActionResult<ClienteViewModel>> Excluir(Guid id)
        {
            var clienteViewModel = _mapper.Map<ClienteViewModel>(await _clienteRepository.ObterClienteEndereco(id));// ObterClienteEndereco(id);

            if (clienteViewModel == null) return NotFound();

            await _clienteService.Remover(id);

            return CustomResponse(clienteViewModel);
        }
        #endregion
    }
}