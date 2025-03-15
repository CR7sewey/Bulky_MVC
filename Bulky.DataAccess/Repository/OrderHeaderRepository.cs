using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bulky.DataAccess.Data;
using Bulky.DataAccess.Migrations;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models.Models;

namespace Bulky.DataAccess.Repository
{
    public class OrderHeaderRepository: Repository<OrderHeader>, IOrderHeaderRepository
    {
        private readonly ApplicationDbContext _db;
        public OrderHeaderRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void update(OrderHeader obj)
        {
            _db.OrderHeaders.Update(obj);
        }

        public void UpdateStatus(int orderHeaderId, string status, string? paymentStatus)
        {
            OrderHeader orderFromDb = _db.OrderHeaders.FirstOrDefault(u => u.Id == orderHeaderId);
            if (orderFromDb != null)
            {
                orderFromDb.OrderStatus = status;
                if (paymentStatus != null)
                {
                    orderFromDb.PaymentStatus = paymentStatus;
                }
            }
        }

        public void UpdateStripePaymentId(int orderHeaderId, string sessionId, string stripePaymentIntentId)
        {
            OrderHeader orderFromDb = _db.OrderHeaders.FirstOrDefault(u => u.Id == orderHeaderId);
            if (orderFromDb != null)
            {
                if (sessionId != null)
                {
                    orderFromDb.SessionId = sessionId;
                }
                
                if (stripePaymentIntentId != null)
                {
                    orderFromDb.PaymentIntendId = stripePaymentIntentId;
                    orderFromDb.PaymentDate = DateTime.Now;
                }
            }
        }
    }
}
