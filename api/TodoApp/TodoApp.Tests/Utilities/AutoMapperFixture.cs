using AutoMapper;
using TodoApp.Api.Mappings;

namespace TodoApp.Tests.Utilities
{
    public class AutoMapperFixture
    {
        public IMapper Mapper { get; }

        public AutoMapperFixture()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<TodoProfile>();
            });

            Mapper = config.CreateMapper();
        }
    }
}
