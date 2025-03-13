using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bulky.Models.Models;

namespace Bulky.DataAccess.Repository.IRepository
{
    public interface IOrderHeaderRepository: IRepository<OrderHeader>
    {
        void update(OrderHeader obj);

        void UpdateStatus(int orderHeaderId, string status, string? paymentStatus = null);

        void UpdateStripePaymentId(int orderHeaderId, string sessionId, string stripePaymentIntentId);
    }
}
