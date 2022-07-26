using ASyncFramework.Domain.Entities;
using ASyncFramework.Domain.Interface.Repository;
using ASyncFramework.Domain.Model.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ASyncFramework.Application.Manager.Service.Command.AddService
{
    public class AddServiceHandler : IRequestHandler<AddServiceCommand, Result>
    {
        private readonly IServiceRepository _ServiceRepository;

        public AddServiceHandler(IServiceRepository serviceRepository)
        {
            _ServiceRepository = serviceRepository;
        }

        public async Task<Result> Handle(AddServiceCommand request, CancellationToken cancellationToken)
        {
            var result = await _ServiceRepository.Add(new ServiceEntity { ArDiscription = request.ArDiscription, CreateDate = DateTime.Now, EnDiscription = request.EnDiscription, ServiceCode = request.ServiceCode, SystemCode = request.SystemCode });

            return new Result(result, null);
        }
    }

    public class AddServiceCommand:IRequest<Result>
    {
        public int ServiceCode { get; set; }
        public string SystemCode { get; set; }
        public string ArDiscription { get; set; }
        public string EnDiscription { get; set; }
    }
}
