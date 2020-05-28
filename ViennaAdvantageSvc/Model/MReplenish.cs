using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VAdvantage.DataBase;
using VAdvantage.Logging;
using VAdvantage.Utility;

namespace ViennaAdvantage.Model
{
    public class MReplenish:X_M_Replenish
    {
        #region Private variables
        // Log		
        private static VLogger s_log = VLogger.GetVLogger(typeof(MReplenish).FullName);

        private static VLogger _log = VLogger.GetVLogger(typeof(MReplenish).FullName);
        //private static CLogger		s_log = CLogger.getCLogger (MStorage.class);
        // Warehouse					
        #endregion

        public static MReplenish Get(Ctx ctx, int M_Warehouse_ID, int M_Product_ID, Trx trxName)
        {
            MReplenish retValue = null;
            String sql = "SELECT * FROM M_Replenish "
                + "WHERE M_Warehouse_ID=" + M_Warehouse_ID + " AND M_Product_ID=" + M_Product_ID + " AND ";
            DataTable dt = null;
            IDataReader idr = null;
            try
            {
                idr = DB.ExecuteReader(sql, null, trxName);
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    retValue = new MReplenish(ctx, dr, trxName);
                }
            }
            catch (Exception ex)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                _log.Log(Level.SEVERE, sql, ex);
            }
            finally
            {
                if (idr != null)
                {
                    idr.Close();
                }
                dt = null;
            }
            return retValue;
        }


        /**
         * 	Load Constructor
         *	@param ctx context
         *	@param dr result set
         */
        public MReplenish(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
            
        }

        public MReplenish(Ctx ctx, int M_Replenish_ID, Trx trxName)
            : base(ctx, M_Replenish_ID, trxName)
        {

        }
    }
}
