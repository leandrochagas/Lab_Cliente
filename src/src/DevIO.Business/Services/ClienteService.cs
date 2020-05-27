using System;
using System.Linq;
using System.Threading.Tasks;
using DevIO.Business.Intefaces;
using DevIO.Business.Models;
using DevIO.Business.Models.Validations;

namespace DevIO.Business.Services
{
    public class ClienteService : BaseService, IClienteService
    {
        private readonly IClienteRepository _clienteRepository;
        private readonly IEnderecoRepository _enderecoRepository;

        public ClienteService(IClienteRepository fornecedorRepository, 
                                 IEnderecoRepository enderecoRepository,
                                 INotificador notificador) : base(notificador)
        {
            _clienteRepository = fornecedorRepository;
            _enderecoRepository = enderecoRepository;
        }

        public async Task<bool> Adicionar(Cliente Cliente)
        {
            if (!ExecutarValidacao(new ClienteValidation(), Cliente) 
                || !ExecutarValidacao(new EnderecoValidation(), Cliente.Endereco)) return false;

            if (_clienteRepository.Buscar(f => f.Documento == Cliente.Documento).Result.Any())
            {
                Notificar("Já existe um Cliente com este documento informado.");
                return false;
            }

            await _clienteRepository.Adicionar(Cliente);
            return true;
        }

        public async Task<bool> Atualizar(Cliente Cliente)
        {
            if (!ExecutarValidacao(new ClienteValidation(), Cliente)) return false;

            if (_clienteRepository.Buscar(f => f.Documento == Cliente.Documento && f.Id != Cliente.Id).Result.Any())
            {
                Notificar("Já existe um Cliente com este documento infomado.");
                return false;
            }

            await _clienteRepository.Atualizar(Cliente);
            return true;
        }


        public async Task<bool> AtualizarEndereco(Endereco Endereco)
        {
            if (!ExecutarValidacao(new EnderecoValidation(), Endereco)) return false;

            if (_enderecoRepository.Buscar(f => f.Id != Endereco.Id && f.Cep == Endereco.Cep).Result.Any())
            {
                Notificar("Já existe este endereço cadastrado em outro cliente");
                return false;
            }
            var endereco = await _enderecoRepository.ObterPorId(Endereco.Id);
         
            endereco.Numero         = Endereco.Numero;
            endereco.Cep            = Endereco.Cep;
            endereco.Logradouro     = Endereco.Logradouro;
            endereco.Bairro         = Endereco.Bairro;
            endereco.Estado         = Endereco.Estado;
            endereco.Complemento    = Endereco.Complemento;
        
            await _enderecoRepository.Atualizar(endereco);
            return true;         
        }

        public async Task<bool> Remover(Guid id)
        {

            var endereco = await _enderecoRepository.ObterEnderecoPorCliente(id);

            if (endereco != null)
            {
                await _enderecoRepository.Remover(endereco.Id);
            }

            await _clienteRepository.Remover(id);
            return true;
        }

        public void Dispose()
        {
            _clienteRepository?.Dispose();
            _enderecoRepository?.Dispose();
        }
    }
}