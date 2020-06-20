using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GameClub.Infrastructure.Abstract;
using GameClub.Infrastructure.Concrete;
using GameClub.Concrete;
using GameClub.Abstract;
using System.Web.Mvc;
using Ninject;

namespace GameClub.Infrastructure
{
    public class NinjectDependencyResolver : IDependencyResolver
    {
        private IKernel kernel;

        public NinjectDependencyResolver()
        {
            kernel = new StandardKernel();
            AddBindings();
        }

        public object GetService(Type serviceType)
        {
            return kernel.TryGet(serviceType);
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return kernel.GetAll(serviceType);
        }

        private void AddBindings()
        {
            kernel.Bind<IAuthProvider>().To<CustomAuthor>();
            kernel.Bind<IMyUserInfo>().To<EFMyUserInfo>();
            kernel.Bind<IAllUserInfo>().To<EFAllUserInfo>();
            kernel.Bind<IGameMember>().To<EFGameMember>();
            kernel.Bind<ISignInfo>().To<EFSignInfo>();
            kernel.Bind<IContribution>().To<EFContribution>();
            kernel.Bind<IMemberGroup>().To<EFMemberGroup>();
            kernel.Bind<IInformMessage>().To<EFInformMessage>();
            kernel.Bind<IPerson>().To<EFPerson>();
            kernel.Bind<IQuestionary>().To<EFQuestionary>();
            kernel.Bind<IArticle>().To<EFArticle>();
            kernel.Bind<IFeedBack>().To<EFFeedBack>();
            kernel.Bind<IUserRecord>().To<EFUserRecord>();
            kernel.Bind<IRecover>().To<EFRecover>();
        }
    }
}