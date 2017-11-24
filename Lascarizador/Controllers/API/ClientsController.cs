using Lascarizador.Core;
using Lascarizador.Dtos;
using Lascarizador.Persistence;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace Lascarizador.Controllers.API
{
    //___/ Controller API para os dados do cliente \____________

    public class ClientsController : ApiController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly LascarizadorDbContext _context;

        public ClientsController()
        {
            _context = new LascarizadorDbContext();
            _unitOfWork = new UnitOfWork(_context);
        }

        // GET /api/clients
        [HttpGet]
        public IEnumerable<ClientDto> GetClients()
        {
            var client = _unitOfWork.Clients.GetAll();

            List<ClientDto> _clientDto = new List<ClientDto>();

            foreach (var c in client)
            {
                var clientDto = new ClientDto();
                clientDto.cpf = c.CPF;
                //clientDto.CreditLimit = Convert.ToInt32(c.CreditLimit * 100);
                clientDto.email = c.Email;
                //clientDto.Id = c.Id;
                clientDto.name = c.Name;
                //clientDto.Saldo = Convert.ToInt32(c.Saldo * 100);

                _clientDto.Add(clientDto);

            }
            return (_clientDto);
        }

        // GET /api/clients/{id}
        [HttpGet]
        public IHttpActionResult GetClient(int id)
        {
            var client = _unitOfWork.Clients.SingleOrDefault(c => c.Id == id);

            if (client == null)
                return NotFound();

            var clientDto = new ClientDto();
            clientDto.cpf = client.CPF;
            //clientDto.CreditLimit = Convert.ToInt32(client.CreditLimit * 100);
            clientDto.email = client.Email;
            //clientDto.Id = client.Id;
            clientDto.name = client.Name;
            //clientDto.Saldo = Convert.ToInt32(client.Saldo * 100);

            //return Ok(Mapper.Map<Client, ClientDto>(client));

            return Ok(clientDto);

        }

        // POST /api/clients
        //[HttpPost]
        //public IHttpActionResult CreateClient(ClientDto clientDto)
        // *** Funcionalidade não implementada ***

        // PUT /api/clients/[id]
        //[HttpPut]
        //public void UpdateClient(int id, ClientDto clientDto)       
        // *** Funcionalidade não implementada ***

        // DELETE /api/clients/[id]
        //[HttpDelete]
        //public void DeleteClient(int id)
        // *** Funcionalidade não implementada ***

    }
}
