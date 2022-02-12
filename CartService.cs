using Sabio.Data;
using Sabio.Data.Providers;
using Sabio.Models.Domain;
using Sabio.Models.Requests.Payments;
using Sabio.Models.Requests.ShoppingCart;
using Sabio.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace Sabio.Services
{
    public class CartService : ICartService
    {
        IDataProvider _data = null;

        public CartService(IDataProvider data)
        {
            _data = data;
        }

        public int AddToCart(int providerServiceId, int userId)
        {
            int id = 0;

            string procName = "[dbo].[ShoppingCart_Insert]";
            _data.ExecuteNonQuery(procName,
                inputParamMapper: delegate (SqlParameterCollection col)
                {
                    col.AddWithValue("@ProviderServiceId", providerServiceId);
                    col.AddWithValue("@CreatedBy", userId);

                    SqlParameter idOut = new SqlParameter("@Id", SqlDbType.Int);
                    idOut.Direction = ParameterDirection.Output;

                    col.Add(idOut);
                },
                returnParameters: delegate (SqlParameterCollection returnCollection)
                {
                    object old = returnCollection["@Id"].Value;
                    Int32.TryParse(old.ToString(), out id);
                });
            return id;
        }


        public List<ShoppingCart> GetByCreatedBy(int userId)
        {
            List<ShoppingCart> cartList = null;
            ShoppingCart aCart = null;

            string procName = "[dbo].[ShoppingCart_GetById]";

            _data.ExecuteCmd(procName, inputParamMapper: delegate (SqlParameterCollection model)
            {
                model.AddWithValue("@CreatedBy", userId);
            },
             singleRecordMapper: delegate (IDataReader reader, short set)
             {

                 aCart = Mapper(reader);

                 if (cartList == null)
                 {
                     cartList = new List<ShoppingCart>();
                 }
                 cartList.Add(aCart);
             });
            return cartList;
        }

        private static ShoppingCart Mapper(IDataReader reader)
        {
            ShoppingCart aCart = new ShoppingCart();

            int startingIndex = 0;

            aCart.Id = reader.GetSafeInt32(startingIndex++);
            aCart.ProviderServiceId = reader.GetSafeInt32(startingIndex++);
            aCart.ProviderId = reader.GetSafeInt32(startingIndex++);
            aCart.Price = reader.GetSafeDecimal(startingIndex++);
            aCart.ServiceName = reader.GetSafeString(startingIndex++);
            aCart.Cpt4Code = reader.GetSafeString(startingIndex++);
            aCart.ServiceType = reader.GetSafeString(startingIndex++);
            aCart.CreatedBy = reader.GetSafeInt32(startingIndex++);

            return aCart;
        }

        public void DeleteItem(int itemId, int userId)
        {
            string procName = "[dbo].[ShoppingCart_Delete_ById]";

            _data.ExecuteNonQuery(procName, delegate (SqlParameterCollection col)
            {
                col.AddWithValue("@Id", itemId);
                col.AddWithValue("@CreatedBy", userId);
            });

        }

        public void DeleteCart(int userId)
        {
            string procName = "[dbo].[ShoppingCart_DeleteAll]";

            _data.ExecuteNonQuery(procName,
            delegate (SqlParameterCollection col)
            {
                col.AddWithValue("@CreatedBy", userId);
            });
        }

    }
}