using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using VAdvantage.DataBase;
using VAdvantage.Model;
using VAdvantage.ProcessEngine;
using VAdvantage.Utility;


namespace VA011.Models
{
    public class ReplenishmentReport
    {
        #region Priate Variables
        string _M_WareSource = "";
        private List<int> createdRecord = new List<int>();
        String _ReplenishmentCreate = null;
        String _DocStatus = null;
        int _C_BPartner_ID = 0;
        int _M_Warehouse_ID = 0;
        int _C_DocType_ID = 0;
        Ctx _ct = null;
        Tuple<String, String, String> aInfo = null;
        String _DocNo = null;
        string _IsOdrPckQty = "Y";
        MPInstance pins = null;
        #endregion Priate Variables

        public DataSet GenerateReplenishmentReport(int M_Warehouse_ID, int C_BPartner_ID, int C_DocType_ID, string DocStatus, string Create, string OrderPack, Ctx ct)
        {
            string sqlProcess = "SELECT AD_Process_ID FROM AD_Process WHERE Value = 'RV_T_Replenish'";
            try
            {
                pins = new MPInstance(ct, Util.GetValueOfInt(DB.ExecuteScalar(sqlProcess)), 1);
                if (pins.Save())
                {

                }
            }
            catch (Exception ex)
            {
                return null;
            }

            DataSet dsRet = null;
            _ReplenishmentCreate = Create;
            _DocStatus = DocStatus;
            _M_Warehouse_ID = M_Warehouse_ID;
            _C_BPartner_ID = C_BPartner_ID;
            _C_DocType_ID = C_DocType_ID;
            _IsOdrPckQty = OrderPack;
            _ct = ct;

            MWarehouse wh = MWarehouse.Get(ct, _M_Warehouse_ID);
            if (wh.Get_ID() == 0)
            {
                throw new Exception("@FillMandatory@ @M_Warehouse_ID@");
            }
            if (wh.GetM_WarehouseSource_ID() > 0)
            {
                _M_WareSource = "M_WarehouseSource_ID = " + Util.GetValueOfString(wh.GetM_WarehouseSource_ID());
            }
            else
            {
                _M_WareSource = null;
            }
            //
            PrepareTable();
            FillTable(wh);

            dsRet = DB.ExecuteDataset("SELECT r.*, p.Name AS Product, w.Name AS Warehouse, sw.Name AS SourceWarehouse,attr.Description AS Attribute " +
                "FROM T_Replenish r INNER JOIN m_Product p ON (p.M_Product_ID = r.M_Product_ID) INNER JOIN M_Warehouse w ON (w.M_Warehouse_ID = r.M_Warehouse_ID) " +
                "LEFT OUTER JOIN M_Warehouse sw ON (sw.M_Warehouse_ID  = r.M_WarehouseSource_ID) LEFT JOIN M_AttributeSetInstance attr ON(attr.M_AttributeSetInstance_ID=r.M_AttributeSetInstance_ID) " +
                "WHERE r.AD_PInstance_ID = " + pins.GetAD_PInstance_ID() + " ORDER BY r.M_Warehouse_ID, r.M_WarehouseSource_ID, r.C_BPartner_ID");

            // int delCount = DB.ExecuteQuery("DELETE FROM T_Replenish WHERE AD_PInstance_ID = " + pins.GetAD_PInstance_ID());

            if (dsRet != null && dsRet.Tables[0] != null)
            {
                foreach (DataColumn column in dsRet.Tables[0].Columns)
                {
                    column.ColumnName = column.ColumnName.ToUpper();
                }
            }
            return dsRet;
            //
            if (Create == null)
            {
                return null;
            }
            //
            //vs
            //MDocType dt = MDocType.Get(ct, _C_DocType_ID);
            //if (!dt.GetDocBaseType().Equals(_ReplenishmentCreate))
            //{
            //    throw new Exception("@C_DocType_ID@=" + dt.GetName() + " <> " + _ReplenishmentCreate);
            //}
            //ve
            if (_C_DocType_ID == 0 || _C_DocType_ID == -1)
            {
            }
            else
            {
                if (Create.Equals("POO"))
                {
                    CreatePO();
                    //Amit
                    if (_DocStatus == "IP" || _DocStatus == "CO")
                    {
                        //if (createdRecord.Count > 0)
                        //{
                        //    int[] array = createdRecord.ToArray();
                        //    for (int k = 0; k < array.Length; k++)
                        //    {
                        //        VAdvantage.Model.MOrder order = new VAdvantage.Model.MOrder(ct, Util.GetValueOfInt(array[k]), null);
                        //        if (_DocStatus == "IP")
                        //        {
                        //            order.SetDocStatus("IP");
                        //            order.Save();
                        //            order.PrepareIt();
                        //            order.Save();
                        //        }
                        //        else if (_DocStatus == "CO")
                        //        {
                        //            order.SetDocStatus("CO");
                        //            order.SetDocAction("CL");
                        //            order.Save();
                        //            order.CompleteIt();
                        //            order.Save();
                        //        }
                        //    }
                        //}
                    }
                    //Amit
                }
                //                else if (Create.Equals("POR"))
                //                {
                //                    CreateRequisition();

                //                }
                //                else if (Create.Equals("MMM"))
                //                {
                //                    CreateMovements();

                //                    //Amit
                //                    if (_DocStatus == "IP" || _DocStatus == "CO")
                //                    {
                //                        if (createdRecord.Count > 0)
                //                        {
                //                            int[] array = createdRecord.ToArray();
                //                            for (int k = 0; k < array.Length; k++)
                //                            {
                //                                VAdvantage.Model.MMovement move = new VAdvantage.Model.MMovement(ct, Util.GetValueOfInt(array[k]), null);
                //                                if (_DocStatus == "IP")
                //                                {
                //                                    move.SetDocStatus("IP");
                //                                    move.Save();
                //                                    move.PrepareIt();
                //                                    move.Save();
                //                                }
                //                                else if (_DocStatus == "CO")
                //                                {
                //                                    move.SetDocStatus("CO");
                //                                    move.SetDocAction("CL");
                //                                    move.Save();
                //                                    move.CompleteIt();
                //                                    move.Save();
                //                                }
                //                            }
                //                        }
                //                    }

                //                    string query = @"DELETE FROM m_movement WHERE m_movement_id IN   (SELECT m_movement_id   FROM M_movement   WHERE m_movement_id NOT IN
                //                                       (SELECT DISTINCT m_movement_id FROM m_movementline     )   )";
                //                    int i = DB.ExecuteQuery(query.ToString(), null, null);
                //                    if (i < 0)
                //                    {
                //                        //log.Info("Inventory Move not deleted where movementline not created ");
                //                    }
                //                    //Amit

                //                }
            }
            return dsRet;
        }

        /// <summary>
        ///	Prepare/Check Replenishment Table
        /// </summary>
        private void PrepareTable()
        {
            String sql;
            int no;

            //  vikas 11/26/2014  2/Dec/2014
            //  This Query Run Only  In case InventoryMove & Purchase Order
            //	Level_Max must be >= Level_Max    
            //if (_ReplenishmentCreate == "MMM" || _ReplenishmentCreate == "POO")
            //{
            //    sql = "UPDATE M_Replenish"
            //       + " SET Level_Max = Level_Min "
            //       + "WHERE Level_Max < Level_Min";
            //    no = DB.ExecuteQuery(sql, null, null);
            //    if (no != 0)
            //    {
            //        //log.Fine("Corrected Max_Level=" + no);
            //    }
            //}


            #region  //  Set  value order min or order pack  in case POO & MMM old
            if (_ReplenishmentCreate == "MMM" || _ReplenishmentCreate == "POO")
            {
                //	Minimum Order should be 1
                //sql = "UPDATE M_Product_PO"
                //    + " SET Order_Min = 1 "
                //    + "WHERE Order_Min IS NULL OR Order_Min < 1";
                //no = DB.ExecuteQuery(sql, null, null);
                //if (no != 0)
                //{
                //    ////log.Fine("Corrected Order Min=" + no);
                //}
                //	Pack should be 1
                sql = "UPDATE M_Product_PO"
                    + " SET Order_Pack = 1 "
                    + "WHERE Order_Pack IS NULL OR Order_Pack < 1";
                no = DB.ExecuteQuery(sql, null, null);
                if (no != 0)
                {
                    // //log.Fine("Corrected Order Pack=" + no);
                }
            }
            #endregion

            # region //	Set  value order min or order pack  in case POR New case
            //  This Query Run Only  In case InventoryMove & Purchase Order (vikas 2/dec/2014) ,Query Updated on 19jan2015
            if (_ReplenishmentCreate == "POR")
            {
                //	Minimum Order should be 1
                //sql = "UPDATE M_Replenish"
                //    + " SET DTD001_MinOrderQty = 1 "
                //    + "WHERE DTD001_MinOrderQty IS NULL OR DTD001_MinOrderQty < 1";
                //no = DB.ExecuteQuery(sql, null, null);
                //if (no != 0)
                //{
                //    ////log.Fine("Corrected Order Min=" + no);
                //}
                if (_ReplenishmentCreate == "POR" & _IsOdrPckQty == "Y")
                {
                    sql = "UPDATE M_Replenish"
                       + " SET DTD001_OrderPackQty = 1 "
                       + "WHERE DTD001_OrderPackQty IS NULL OR DTD001_OrderPackQty < 1";
                    no = DB.ExecuteQuery(sql, null, null);
                    if (no != 0)
                    {
                        //log.Fine("Corrected Order Pack=" + no);
                    }
                }
            }
            #endregion

            //	Set Current Vendor where only one vendor
            sql = "UPDATE M_Product_PO p"
                + " SET IsCurrentVendor='Y' "
                + "WHERE IsCurrentVendor<>'Y'"
                //jz groupby problem + " AND EXISTS (SELECT * FROM M_Product_PO pp "
                + " AND EXISTS (SELECT 1 FROM M_Product_PO pp "
                    + "WHERE p.M_Product_ID=pp.M_Product_ID "
                    + "GROUP BY pp.M_Product_ID "
                    + "HAVING COUNT(*) = 1)";
            no = DB.ExecuteQuery(sql, null, null);
            if (no != 0)
            {
                ////log.Fine("Corrected CurrentVendor(Y)=" + no);
            }

            //	More then one current vendor
            sql = "UPDATE M_Product_PO p"
                + " SET IsCurrentVendor='N' "
                + "WHERE IsCurrentVendor = 'Y'"
                //jz + " AND EXISTS (SELECT * FROM M_Product_PO pp "
                + " AND EXISTS (SELECT 1 FROM M_Product_PO pp "
                    + "WHERE p.M_Product_ID=pp.M_Product_ID AND pp.IsCurrentVendor='Y' "
                    + "GROUP BY pp.M_Product_ID "
                    + "HAVING COUNT(*) > 1)";
            no = DB.ExecuteQuery(sql, null, null);
            if (no != 0)
            {
                ////log.Fine("Corrected CurrentVendor(N)=" + no);
            }

            //	Just to be sure
            sql = "DELETE FROM T_Replenish WHERE AD_PInstance_ID=" + pins.GetAD_PInstance_ID();
            no = DB.ExecuteQuery(sql, null, null);
            if (no != 0)
            {
                ////log.Fine("Delete Existing Temp=" + no);
            }
        }	//	prepareTable

        /// <summary>
        /// Fill Table
        /// </summary>
        /// <param name="wh">warehouse</param>
        private void FillTable(MWarehouse wh)
        {
            DataSet ds = null;
            //string sqlRep = "SELECT * FROM T_Replenish WHERE AD_PInstance_ID = " + pins.GetAD_PInstance_ID();

            String sql = null;
            int no = 0;
            //#region POO & MMM Case orderpack or min qty consider from  purchasing tab  20 jan 2015 v
            //if (_ReplenishmentCreate == "MMM" || _ReplenishmentCreate == "POO")

            #region POO  Case orderpack or min qty consider from  purchasing tab  20 jan 2015 v
            if (_ReplenishmentCreate == "POO")
            {
                // In Serial No,  i put Min order qty from replenish as a dummy data
                sql = "INSERT INTO T_Replenish "
                + "(AD_PInstance_ID, M_Warehouse_ID,DocStatus, M_Product_ID, AD_Client_ID, AD_Org_ID,"
                + " ReplenishType, M_AttributeSetInstance_ID, Level_Min, Level_Max, QtyOnHand,QtyReserved,QtyOrdered,"
                + " C_BPartner_ID, Order_Min, Order_Pack, QtyToOrder, ReplenishmentCreate , Serial_No) "
                + "SELECT " + pins.GetAD_PInstance_ID()
                    + ", r.M_Warehouse_ID,'" + _DocStatus + "', r.M_Product_ID, r.AD_Client_ID, r.AD_Org_ID,"
                + " r.ReplenishType, r.M_AttributeSetInstance_ID, r.Level_Min, r.Level_Max, 0,0,0,"
                + " po.C_BPartner_ID, po.Order_Min, CASE WHEN NVL(po.Order_Pack, 0) > 0 THEN po.Order_Pack ELSE NVL(r.DTD001_OrderPackQty, 0) END AS Order_Pack, 0, ";
                if (_ReplenishmentCreate == null)
                {
                    sql += "null";
                }
                else
                {
                    sql += "'" + _ReplenishmentCreate + "'";
                }
                sql += " , NVL(r.DTD001_MinOrderQty,0)";
                sql += " FROM M_Replenish r"
                    + " INNER JOIN M_Product_PO po ON (r.M_Product_ID=po.M_Product_ID) "
                    + "WHERE po.IsCurrentVendor='Y'"	//	Only Current Vendor
                    + " AND r.ReplenishType<>'0'"
                    + " AND po.IsActive='Y' AND r.IsActive='Y'"
                    + " AND r.M_Warehouse_ID=" + _M_Warehouse_ID;
                if (_C_BPartner_ID != 0 && _C_BPartner_ID != -1)
                {
                    sql += " AND po.C_BPartner_ID=" + _C_BPartner_ID;
                }
                no = DB.ExecuteQuery(sql, null, null);
                // no = DB.ExecuteQuery(sql, null, null);
                //log.Finest(sql);
                //log.Fine("Insert (1) #" + no);

                if (_C_BPartner_ID == 0 || _C_BPartner_ID == -1)
                {
                    sql = "INSERT INTO T_Replenish "
                        + "(AD_PInstance_ID, M_Warehouse_ID,DocStatus, M_Product_ID, AD_Client_ID, AD_Org_ID,"
                        + " ReplenishType, Level_Min, Level_Max,"
                        + " C_BPartner_ID, Order_Min, Order_Pack, QtyToOrder, ReplenishmentCreate) "
                        + "SELECT " + pins.GetAD_PInstance_ID()
                        + ", r.M_Warehouse_ID,'" + _DocStatus + "', r.M_Product_ID, r.AD_Client_ID, r.AD_Org_ID,"
                        + " r.ReplenishType, r.Level_Min, r.Level_Max,"
                        //jz + " null, 1, 1, 0, ";
                        + DB.NULL("I", Types.VARCHAR)
                        + " , 1, 1, 0, ";
                    if (_ReplenishmentCreate == null)
                    {
                        sql += "null";
                    }
                    else
                    {
                        sql += "'" + _ReplenishmentCreate + "'";
                    }
                    sql += " FROM M_Replenish r "
                        + "WHERE r.ReplenishType<>'0' AND r.IsActive='Y'"
                        + " AND r.M_Warehouse_ID=" + _M_Warehouse_ID
                        + " AND NOT EXISTS (SELECT * FROM T_Replenish t "
                            + "WHERE r.M_Product_ID=t.M_Product_ID"
                            + " AND AD_PInstance_ID=" + pins.GetAD_PInstance_ID() + ")";

                    no = DB.ExecuteQuery(sql, null, null);
                    // no = DB.ExecuteQuery(sql, null, null);
                    //log.Fine("Insert (BP) #" + no);
                }
            }
            #endregion

            #region MMM Case orderpack or min qty consider from  purchasing tab  20 jan 2015 v
            if (_ReplenishmentCreate == "MMM")
            {
                sql = "INSERT INTO T_Replenish "
                + "(AD_PInstance_ID, M_Warehouse_ID,DocStatus, M_Product_ID, AD_Client_ID, AD_Org_ID,"
                + " ReplenishType, M_AttributeSetInstance_ID,Level_Min, Level_Max, QtyOnHand,QtyReserved,QtyOrdered,"
                + " C_BPartner_ID, Order_Min, Order_Pack, QtyToOrder, ReplenishmentCreate) "
                + "SELECT " + pins.GetAD_PInstance_ID()
                    + ", r.M_Warehouse_ID,'" + _DocStatus + "', r.M_Product_ID, r.AD_Client_ID, r.AD_Org_ID,"
                + " r.ReplenishType, r.M_AttributeSetInstance_ID, r.Level_Min, r.Level_Max, 0,0,0,"
                + " po.C_BPartner_ID, po.Order_Min, CASE WHEN NVL(po.Order_Pack, 0) > 0 THEN po.Order_Pack ELSE NVL(r.DTD001_OrderPackQty, 0) END AS Order_Pack, 0,";
                if (_ReplenishmentCreate == null)
                {
                    sql += "null";
                }
                else
                {
                    sql += "'" + _ReplenishmentCreate + "'";
                }
                sql += " FROM M_Replenish r"
                    + " INNER JOIN M_Product_PO po ON (r.M_Product_ID=po.M_Product_ID) "
                    + "WHERE po.IsCurrentVendor='Y'"	//	Only Current Vendor
                    + " AND r.ReplenishType<>'0'"
                    + " AND po.IsActive='Y' AND r.IsActive='Y'"
                    + " AND r.M_Warehouse_ID=" + _M_Warehouse_ID;
                if (_C_BPartner_ID != 0 && _C_BPartner_ID != -1)
                {
                    sql += " AND po.C_BPartner_ID=" + _C_BPartner_ID;
                }
                no = DB.ExecuteQuery(sql, null, null);
                // no = DB.ExecuteQuery(sql, null, null);
                // log.Finest(sql);
                //log.Fine("Insert (1) #" + no);

                if (_C_BPartner_ID == 0 || _C_BPartner_ID == -1)
                {
                    sql = "INSERT INTO T_Replenish "
                        + "(AD_PInstance_ID, M_Warehouse_ID,DocStatus, M_Product_ID, AD_Client_ID, AD_Org_ID,"
                        + " ReplenishType, M_AttributeSetInstance_ID, Level_Min, Level_Max,"
                        + " C_BPartner_ID, Order_Min, Order_Pack, QtyToOrder, ReplenishmentCreate) "
                        + "SELECT " + pins.GetAD_PInstance_ID()
                        + ", r.M_Warehouse_ID,'" + _DocStatus + "', r.M_Product_ID, r.AD_Client_ID, r.AD_Org_ID,"
                        + " r.ReplenishType,r.M_AttributeSetInstance_ID, r.Level_Min, r.Level_Max,"
                        //jz + " null, 1, 1, 0, ";
                        + DB.NULL("I", Types.VARCHAR)
                        + " , 1, 1, 0, ";
                    if (_ReplenishmentCreate == null)
                    {
                        sql += "null";
                    }
                    else
                    {
                        sql += "'" + _ReplenishmentCreate + "'";
                    }
                    sql += " FROM M_Replenish r "
                        + "WHERE r.ReplenishType<>'0' AND r.IsActive='Y'"
                        + " AND r.M_Warehouse_ID=" + _M_Warehouse_ID
                        + " AND NOT EXISTS (SELECT * FROM T_Replenish t "
                            + "WHERE r.M_Product_ID=t.M_Product_ID"
                            + " AND AD_PInstance_ID=" + pins.GetAD_PInstance_ID() + ")";

                    no = DB.ExecuteQuery(sql, null, null);
                    // no = DB.ExecuteQuery(sql, null, null);
                    //log.Fine("Insert (BP) #" + no);
                }
            }
            #endregion

            #region POR Requisition Case orderpack or min qty consider from  product Replenish Tab  20 jan 2015 v
            if (_ReplenishmentCreate == "POR")
            {
                sql = "INSERT INTO T_Replenish "
                + "(AD_PInstance_ID, M_Warehouse_ID,DocStatus, M_Product_ID, AD_Client_ID, AD_Org_ID,"
                + " ReplenishType,M_AttributeSetInstance_ID, Level_Min, Level_Max, QtyOnHand,QtyReserved,QtyOrdered,"
                + " C_BPartner_ID, Order_Min, Order_Pack, QtyToOrder, ReplenishmentCreate) "
                + "SELECT " + pins.GetAD_PInstance_ID()
                    + ", r.M_Warehouse_ID,'" + _DocStatus + "', r.M_Product_ID, r.AD_Client_ID, r.AD_Org_ID,"
                + " r.ReplenishType,r.M_AttributeSetInstance_ID, r.Level_Min, r.Level_Max, 0,0,0,"
                + " po.C_BPartner_ID,NVL(r.DTD001_MinOrderQty,0), NVL(r.DTD001_OrderPackQty,0), 0, ";
                if (_ReplenishmentCreate == null)
                {
                    sql += "null";
                }
                else
                {
                    sql += "'" + _ReplenishmentCreate + "'";
                }
                sql += " FROM M_Replenish r"
                    + " INNER JOIN M_Product_PO po ON (r.M_Product_ID=po.M_Product_ID) "
                     + "WHERE po.IsCurrentVendor='Y'"	//	Only Current Vendor
                    + " AND r.ReplenishType<>'0'"

                    // + "WHERE r.ReplenishType<>'0' "	//	Only Current Vendor
                    + " AND po.IsActive='Y' AND r.IsActive='Y'   AND  ( r.Level_Min<> 0 OR r.Level_Max <> 0 )   "  // max or min level 0 qty not consider
                    + " AND r.M_Warehouse_ID=" + _M_Warehouse_ID;
                if (_C_BPartner_ID != 0 && _C_BPartner_ID != -1)
                {
                    sql += " AND po.C_BPartner_ID=" + _C_BPartner_ID;
                }
                no = DB.ExecuteQuery(sql, null, null);
                //  no = DB.ExecuteQuery(sql, null, null);
                //log.Finest(sql);
                // //log.Fine("Insert (1) #" + no);

                ds = DB.ExecuteDataset(sql, null, null);

                if (_C_BPartner_ID == 0 || _C_BPartner_ID == -1)
                {
                    sql = "INSERT INTO T_Replenish "
                        + "(AD_PInstance_ID, M_Warehouse_ID,DocStatus, M_Product_ID, AD_Client_ID, AD_Org_ID,"
                        + " ReplenishType, M_AttributeSetInstance_ID, Level_Min, Level_Max,"
                        + " C_BPartner_ID, Order_Min, Order_Pack, QtyToOrder, ReplenishmentCreate) "
                        + "SELECT " + pins.GetAD_PInstance_ID()
                        + ", r.M_Warehouse_ID,'" + _DocStatus + "', r.M_Product_ID, r.AD_Client_ID, r.AD_Org_ID,"
                        + " r.ReplenishType, r.M_AttributeSetInstance_ID, r.Level_Min, r.Level_Max,"
                        //jz + " null, 1, 1, 0, ";
                        + DB.NULL("I", Types.VARCHAR)
                        + " , NVL(r.DTD001_MinOrderQty,0), NVL(r.DTD001_OrderPackQty,0), 0, ";
                    if (_ReplenishmentCreate == null)
                    {
                        sql += "null";
                    }
                    else
                    {
                        sql += "'" + _ReplenishmentCreate + "'";
                    }
                    sql += " FROM M_Replenish r "
                        + "WHERE r.ReplenishType<>'0' AND r.IsActive='Y'  AND  ( r.Level_Min<> 0 OR r.Level_Max <> 0 )  "
                        + " AND r.M_Warehouse_ID=" + _M_Warehouse_ID
                        + " AND NOT EXISTS (SELECT * FROM T_Replenish t "
                            + "WHERE r.M_Product_ID=t.M_Product_ID"
                            + " AND AD_PInstance_ID=" + pins.GetAD_PInstance_ID() + ")";

                    no = DB.ExecuteQuery(sql, null, null);
                    //  no = DB.ExecuteQuery(sql, null, null);
                    ////log.Fine("Insert (BP) #" + no);
                }
            }
            #endregion


            //dtd
            int _CountDTD001 = Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(AD_MODULEINFO_ID) FROM AD_MODULEINFO WHERE PREFIX='DTD001_'"));
            if (_CountDTD001 > 0)
            {
                sql = "UPDATE T_Replenish t SET "
                //+ "QtyOnHand = (SELECT COALESCE(SUM(QtyOnHand),0) FROM M_Storage s, M_Locator l WHERE t.M_Product_ID=s.M_Product_ID"
                //    + " AND l.M_Locator_ID=s.M_Locator_ID AND l.M_Warehouse_ID=t.M_Warehouse_ID),"
                //+ "QtyReserved = (SELECT COALESCE(SUM(QtyReserved),0) FROM M_Storage s, M_Locator l WHERE t.M_Product_ID=s.M_Product_ID"
                //    + " AND l.M_Locator_ID=s.M_Locator_ID AND l.M_Warehouse_ID=t.M_Warehouse_ID),"
                //+ "QtyOrdered = (SELECT COALESCE(SUM(QtyOrdered),0) FROM M_Storage s, M_Locator l WHERE t.M_Product_ID=s.M_Product_ID"
                //    + " AND l.M_Locator_ID=s.M_Locator_ID AND l.M_Warehouse_ID=t.M_Warehouse_ID),"
                //+ "DTD001_QtyReserved = (SELECT COALESCE(SUM(DTD001_QtyReserved),0) FROM M_Storage s, M_Locator l WHERE t.M_Product_ID=s.M_Product_ID"
                //+ " AND l.M_Locator_ID=s.M_Locator_ID AND l.M_Warehouse_ID=t.M_Warehouse_ID),"
                //    //v3
                //+ "DTD001_SourceReserve = (SELECT COALESCE(SUM(DTD001_SourceReserve),0) FROM M_Storage s, M_Locator l WHERE t.M_Product_ID=s.M_Product_ID"
                //    + " AND l.M_Locator_ID=s.M_Locator_ID AND l.M_Warehouse_ID=t.M_Warehouse_ID)";
                + "QtyOnHand = CASE WHEN (t.M_Attributesetinstance_ID > 0) THEN (SELECT COALESCE(SUM(QtyOnHand),0) FROM M_Storage s, M_Locator l WHERE t.M_Product_ID=s.M_Product_ID"
                    + " AND l.M_Locator_ID=s.M_Locator_ID AND l.M_Warehouse_ID=t.M_Warehouse_ID AND t.M_Attributesetinstance_ID=s.M_Attributesetinstance_ID)"
                    + " ELSE (SELECT COALESCE(SUM(QtyOnHand),0) FROM M_Storage s, M_Locator l WHERE t.M_Product_ID=s.M_Product_ID"
                    + " AND l.M_Locator_ID=s.M_Locator_ID AND l.M_Warehouse_ID=t.M_Warehouse_ID AND s.M_Attributesetinstance_ID NOT IN ("
                    + " SELECT M_AttributeSetInstance_ID FROM T_Replenish WHERE M_Product_ID = t.M_Product_ID AND M_AttributeSetInstance_ID > 0 AND AD_PInstance_ID=" + pins.GetAD_PInstance_ID() + ")) END,"
                + "QtyReserved = CASE WHEN (t.M_Attributesetinstance_ID > 0) THEN (SELECT COALESCE(SUM(QtyReserved),0) FROM M_Storage s, M_Locator l WHERE t.M_Product_ID=s.M_Product_ID"
                    + " AND l.M_Locator_ID=s.M_Locator_ID AND l.M_Warehouse_ID=t.M_Warehouse_ID AND t.M_Attributesetinstance_ID=s.M_Attributesetinstance_ID)"
                    + " ELSE (SELECT COALESCE(SUM(QtyReserved),0) FROM M_Storage s, M_Locator l WHERE t.M_Product_ID=s.M_Product_ID"
                    + " AND l.M_Locator_ID=s.M_Locator_ID AND l.M_Warehouse_ID=t.M_Warehouse_ID AND s.M_Attributesetinstance_ID NOT IN ("
                    + " SELECT M_AttributeSetInstance_ID FROM T_Replenish WHERE M_Product_ID = t.M_Product_ID AND M_AttributeSetInstance_ID > 0 AND AD_PInstance_ID=" + pins.GetAD_PInstance_ID() + ")) END,"
                + "QtyOrdered = CASE WHEN (t.M_Attributesetinstance_ID > 0) THEN (SELECT COALESCE(SUM(QtyOrdered),0) FROM M_Storage s, M_Locator l WHERE t.M_Product_ID=s.M_Product_ID"
                    + " AND l.M_Locator_ID=s.M_Locator_ID AND l.M_Warehouse_ID=t.M_Warehouse_ID AND t.M_Attributesetinstance_ID=s.M_Attributesetinstance_ID)"
                    + " ELSE (SELECT COALESCE(SUM(QtyOrdered),0) FROM M_Storage s, M_Locator l WHERE t.M_Product_ID=s.M_Product_ID"
                    + " AND l.M_Locator_ID=s.M_Locator_ID AND l.M_Warehouse_ID=t.M_Warehouse_ID AND s.M_Attributesetinstance_ID NOT IN ("
                    + " SELECT M_AttributeSetInstance_ID FROM T_Replenish WHERE M_Product_ID = t.M_Product_ID AND M_AttributeSetInstance_ID > 0 AND AD_PInstance_ID=" + pins.GetAD_PInstance_ID() + ")) END,"
                + "DTD001_QtyReserved = CASE WHEN (t.M_Attributesetinstance_ID > 0) THEN (SELECT COALESCE(SUM(DTD001_QtyReserved),0) FROM M_Storage s, M_Locator l WHERE t.M_Product_ID=s.M_Product_ID"
                   + " AND l.M_Locator_ID=s.M_Locator_ID AND l.M_Warehouse_ID=t.M_Warehouse_ID AND t.M_Attributesetinstance_ID=s.M_Attributesetinstance_ID)"
                   + " ELSE (SELECT COALESCE(SUM(DTD001_QtyReserved),0) FROM M_Storage s, M_Locator l WHERE t.M_Product_ID=s.M_Product_ID"
                   + " AND l.M_Locator_ID=s.M_Locator_ID AND l.M_Warehouse_ID=t.M_Warehouse_ID AND s.M_Attributesetinstance_ID NOT IN ("
                    + " SELECT M_AttributeSetInstance_ID FROM T_Replenish WHERE M_Product_ID = t.M_Product_ID AND M_AttributeSetInstance_ID > 0 AND AD_PInstance_ID=" + pins.GetAD_PInstance_ID() + ")) END,"
                //v3
                + "DTD001_SourceReserve = CASE WHEN (t.M_Attributesetinstance_ID > 0) THEN (SELECT COALESCE(SUM(DTD001_SourceReserve),0) FROM M_Storage s, M_Locator l WHERE t.M_Product_ID=s.M_Product_ID"
                    + " AND l.M_Locator_ID=s.M_Locator_ID AND l.M_Warehouse_ID=t.M_Warehouse_ID AND t.M_Attributesetinstance_ID=s.M_Attributesetinstance_ID)"
                    + " ELSE (SELECT COALESCE(SUM(DTD001_SourceReserve),0) FROM M_Storage s, M_Locator l WHERE t.M_Product_ID=s.M_Product_ID"
                    + " AND l.M_Locator_ID=s.M_Locator_ID AND l.M_Warehouse_ID=t.M_Warehouse_ID AND s.M_Attributesetinstance_ID NOT IN ("
                    + " SELECT M_AttributeSetInstance_ID FROM T_Replenish WHERE M_Product_ID = t.M_Product_ID AND M_AttributeSetInstance_ID > 0 AND AD_PInstance_ID=" + pins.GetAD_PInstance_ID() + ")) END";
            }
            else
            {
                sql = "UPDATE T_Replenish t SET "
                //+ "QtyOnHand = (SELECT COALESCE(SUM(QtyOnHand),0) FROM M_Storage s, M_Locator l WHERE t.M_Product_ID=s.M_Product_ID"
                //    + " AND l.M_Locator_ID=s.M_Locator_ID AND l.M_Warehouse_ID=t.M_Warehouse_ID),"
                //+ "QtyReserved = (SELECT COALESCE(SUM(QtyReserved),0) FROM M_Storage s, M_Locator l WHERE t.M_Product_ID=s.M_Product_ID"
                //    + " AND l.M_Locator_ID=s.M_Locator_ID AND l.M_Warehouse_ID=t.M_Warehouse_ID),"
                //+ "QtyOrdered = (SELECT COALESCE(SUM(QtyOrdered),0) FROM M_Storage s, M_Locator l WHERE t.M_Product_ID=s.M_Product_ID"
                //    + " AND l.M_Locator_ID=s.M_Locator_ID AND l.M_Warehouse_ID=t.M_Warehouse_ID)";
                + "QtyOnHand = CASE WHEN (t.M_Attributesetinstance_ID > 0) THEN (SELECT COALESCE(SUM(QtyOnHand),0) FROM M_Storage s, M_Locator l WHERE t.M_Product_ID=s.M_Product_ID"
                    + " AND l.M_Locator_ID=s.M_Locator_ID AND l.M_Warehouse_ID=t.M_Warehouse_ID AND t.M_Attributesetinstance_ID=s.M_Attributesetinstance_ID)"
                    + " ELSE (SELECT COALESCE(SUM(QtyOnHand),0) FROM M_Storage s, M_Locator l WHERE t.M_Product_ID=s.M_Product_ID"
                    + " AND l.M_Locator_ID=s.M_Locator_ID AND l.M_Warehouse_ID=t.M_Warehouse_ID AND s.M_Attributesetinstance_ID NOT IN ("
                    + " SELECT M_AttributeSetInstance_ID FROM T_Replenish WHERE M_Product_ID = t.M_Product_ID AND M_AttributeSetInstance_ID > 0 AND AD_PInstance_ID=" + pins.GetAD_PInstance_ID() + ")) END,"
                + "QtyReserved = CASE WHEN (t.M_Attributesetinstance_ID > 0) THEN (SELECT COALESCE(SUM(QtyReserved),0) FROM M_Storage s, M_Locator l WHERE t.M_Product_ID=s.M_Product_ID"
                    + " AND l.M_Locator_ID=s.M_Locator_ID AND l.M_Warehouse_ID=t.M_Warehouse_ID AND t.M_Attributesetinstance_ID=s.M_Attributesetinstance_ID)"
                    + " ELSE (SELECT COALESCE(SUM(QtyReserved),0) FROM M_Storage s, M_Locator l WHERE t.M_Product_ID=s.M_Product_ID"
                    + " AND l.M_Locator_ID=s.M_Locator_ID AND l.M_Warehouse_ID=t.M_Warehouse_ID AND s.M_Attributesetinstance_ID NOT IN ("
                    + " SELECT M_AttributeSetInstance_ID FROM T_Replenish WHERE M_Product_ID = t.M_Product_ID AND M_AttributeSetInstance_ID > 0 AND AD_PInstance_ID=" + pins.GetAD_PInstance_ID() + ")) END,"
                + "QtyOrdered = CASE WHEN (t.M_Attributesetinstance_ID > 0) THEN (SELECT COALESCE(SUM(QtyOrdered),0) FROM M_Storage s, M_Locator l WHERE t.M_Product_ID=s.M_Product_ID"
                    + " AND l.M_Locator_ID=s.M_Locator_ID AND l.M_Warehouse_ID=t.M_Warehouse_ID AND t.M_Attributesetinstance_ID=s.M_Attributesetinstance_ID)"
                    + " ELSE (SELECT COALESCE(SUM(QtyOrdered),0) FROM M_Storage s, M_Locator l WHERE t.M_Product_ID=s.M_Product_ID"
                    + " AND l.M_Locator_ID=s.M_Locator_ID AND l.M_Warehouse_ID=t.M_Warehouse_ID AND s.M_Attributesetinstance_ID NOT IN ("
                    + " SELECT M_AttributeSetInstance_ID FROM T_Replenish WHERE M_Product_ID = t.M_Product_ID AND M_AttributeSetInstance_ID > 0 AND AD_PInstance_ID=" + pins.GetAD_PInstance_ID() + ")) END,";
            }
            //dtd
            if (_C_DocType_ID != 0 && _C_DocType_ID != -1)
            {
                sql += ", C_DocType_ID=" + _C_DocType_ID;
            }
            sql += " WHERE AD_PInstance_ID=" + pins.GetAD_PInstance_ID();
            no = DB.ExecuteQuery(sql, null, null);
            if (no != 0)
            {
                ////log.Fine("Update #" + no);
            }

            //	Delete inactive products and replenishments
            sql = "DELETE FROM T_Replenish r "
                + "WHERE (EXISTS (SELECT * FROM M_Product p "
                    + "WHERE p.M_Product_ID=r.M_Product_ID AND p.IsActive='N')"
                + " OR EXISTS (SELECT * FROM M_Replenish rr "
                    + " WHERE rr.M_Product_ID=r.M_Product_ID AND rr.M_Warehouse_ID=r.M_Warehouse_ID AND rr.IsActive='N'))"
                + " AND AD_PInstance_ID=" + pins.GetAD_PInstance_ID();
            no = DB.ExecuteQuery(sql, null, null);
            if (no != 0)
            {
                //log.Fine("Delete Inactive=" + no);
            }

            //	Ensure Data consistency
            sql = "UPDATE T_Replenish SET QtyOnHand = 0 WHERE QtyOnHand IS NULL";
            no = DB.ExecuteQuery(sql, null, null);
            sql = "UPDATE T_Replenish SET QtyReserved = 0 WHERE QtyReserved IS NULL";
            no = DB.ExecuteQuery(sql, null, null);
            sql = "UPDATE T_Replenish SET QtyOrdered = 0 WHERE QtyOrdered IS NULL";
            no = DB.ExecuteQuery(sql, null, null);

            //	Set Minimum / Maximum Maintain Level
            //	X_M_Replenish.REPLENISHTYPE_ReorderBelowMinimumLevel
            sql = "UPDATE T_Replenish"
                //+ " SET QtyToOrder = Level_Min - QtyOnHand + QtyReserved - QtyOrdered - DTD001_QtyReserved  "  // DTD001_QtyReserved  as requisition reserver quantity
                + " SET QtyToOrder = Level_Min - QtyOnHand + QtyReserved + DTD001_SourceReserve - QtyOrdered - DTD001_QtyReserved  "  // DTD001_QtyReserved  as requisition reserver quantity
                + "WHERE ReplenishType='1'"
                + " AND AD_PInstance_ID=" + pins.GetAD_PInstance_ID();
            no = DB.ExecuteQuery(sql, null, null);
            if (no != 0)
            {
                ////log.Fine("Update Type-1=" + no);
            }
            //
            //	X_M_Replenish.REPLENISHTYPE_MaintainMaximumLevel
            sql = "UPDATE T_Replenish"
                //+ " SET QtyToOrder = Level_Max - QtyOnHand + QtyReserved - QtyOrdered - DTD001_QtyReserved  "
                + " SET QtyToOrder = Level_Max - QtyOnHand + QtyReserved + DTD001_SourceReserve - QtyOrdered - DTD001_QtyReserved  "
                + "WHERE ReplenishType='2'"
                + " AND AD_PInstance_ID=" + pins.GetAD_PInstance_ID();
            no = DB.ExecuteQuery(sql, null, null);
            if (no != 0)
            {
                ////log.Fine("Update Type-2=" + no);
            }

            //	Delete rows where nothing to order
            sql = "DELETE FROM T_Replenish "
                + "WHERE QtyToOrder < 1"
                + " AND AD_PInstance_ID=" + pins.GetAD_PInstance_ID();
            no = DB.ExecuteQuery(sql, null, null);
            if (no != 0)
            {
                ////log.Fine("Delete No QtyToOrder=" + no);
            }

            ////	Minimum Order Quantity
            //sql = "UPDATE T_Replenish"
            //    + " SET QtyToOrder = Order_Min "
            //    + "WHERE QtyToOrder < Order_Min"
            //    + " AND AD_PInstance_ID=" + GetAD_PInstance_ID();
            //no = DataBase.DB.ExecuteQuery(sql, null, null);
            //if (no != 0)
            //{
            //    //log.Fine("Set MinOrderQty=" + no);
            //}

            //	Even dividable by Pack
            //  Vikas 19/jan/2015 Consider order pack qty if check parameter true IN REQUISITION CASE
            //commented By Lokesh Chauhan 
            if ((_ReplenishmentCreate == "POR" || _ReplenishmentCreate == "POO") & _IsOdrPckQty == "Y")   // N
            //if (_ReplenishmentCreate != "MMM" & _IsOdrPckQty == "Y")   // N
            {
                sql = "UPDATE T_Replenish"
                    + " SET QtyToOrder = QtyToOrder - MOD(QtyToOrder, Order_Pack) + Order_Pack "
                    + "WHERE MOD(QtyToOrder, Order_Pack) <> 0"
                    + " AND AD_PInstance_ID=" + pins.GetAD_PInstance_ID();
                no = DB.ExecuteQuery(sql, null, null);
                sql = "UPDATE T_Replenish Set DTD001_IsOrderPckQty='Y' where   AD_PInstance_ID=" + pins.GetAD_PInstance_ID();
                no = DB.ExecuteQuery(sql, null, null);
                if (no != 0)
                {
                    //log.Fine("Set OrderPackQty=" + no);
                }
            }

            // vikas Set qty to order maximum level where qty to order Greater  20jan 2015 
            //  sql = "  update t_replenish set  QtyToOrder= level_max  WHERE  level_max >0 and qtytoorder > level_max AND  ad_pinstance_id =" + GetAD_PInstance_ID();
            // no = DB.ExecuteQuery(sql, null, null);

            // set  Minimum order qty if qty to order less than v
            //sql = "  update t_replenish set  QtyToOrder= order_min  WHERE  order_min >0 and qtytoorder < order_min AND  ad_pinstance_id =" + pins.GetAD_PInstance_ID();
            //no = DB.ExecuteQuery(sql, null, null);

            // change By Amit on behalf of Vaibhav 19-10-2015
            if (_ReplenishmentCreate == "MMM")
            {
                sql = "  update t_replenish set  QtyToOrder= order_min  WHERE  order_min >0 and qtytoorder < order_min AND  ad_pinstance_id =" + pins.GetAD_PInstance_ID();
                no = DB.ExecuteQuery(sql, null, null);
            }

            if (_ReplenishmentCreate == "POR")
            {
                // change qtyOrder when if the minimum order is greater than zero
                sql = "  UPDATE t_replenish SET  QtyToOrder= order_min + QtyToOrder  WHERE  order_min >0 AND ReplenishType='1' AND  ad_pinstance_id =" + pins.GetAD_PInstance_ID();
                no = DB.ExecuteQuery(sql, null, null);

                // change qtyOrder when if the minimum order is equal to zero
                sql = "  UPDATE t_replenish SET  QtyToOrder=  QtyToOrder  WHERE  order_min = 0 AND ReplenishType='1' AND  ad_pinstance_id =" + pins.GetAD_PInstance_ID();
                no = DB.ExecuteQuery(sql, null, null);

                // Maintain Minimum Order qty when replenishmnt set maximam level
                sql = "  UPDATE t_replenish SET  QtyToOrder= order_min  WHERE  order_min > QtyToOrder AND ReplenishType='2' AND  ad_pinstance_id =" + pins.GetAD_PInstance_ID();
                no = DB.ExecuteQuery(sql, null, null);
            }

            if (_ReplenishmentCreate == "POO")
            {
                //change qtyOrder when if the minimum order qty at purchasing is greater than equal to order qty at Replenish Tab then take purchasing qty
                sql = "  UPDATE t_replenish SET  QtyToOrder=  QtyToOrder + order_min  WHERE  order_min >= serial_no AND ReplenishType='1' AND  ad_pinstance_id =" + pins.GetAD_PInstance_ID();
                no = DB.ExecuteQuery(sql, null, null);

                //change qtyOrder when if the minimum order qty at purchasing is less than order qty at Replenish Tab then take replenish qty
                sql = "  UPDATE t_replenish SET  QtyToOrder=  QtyToOrder + serial_no  WHERE  order_min < serial_no AND ReplenishType='1' AND  ad_pinstance_id =" + pins.GetAD_PInstance_ID();
                no = DB.ExecuteQuery(sql, null, null);

                //change qtyOrder when if the minimum order qty at purchasing is greater than equal to order qty at Replenish Tab then take purchasing qty
                sql = "  UPDATE t_replenish SET  QtyToOrder= order_min  WHERE  order_min >= serial_no  AND order_min > QtyToOrder  AND ReplenishType='2' AND  ad_pinstance_id =" + pins.GetAD_PInstance_ID();
                no = DB.ExecuteQuery(sql, null, null);

                //change qtyOrder when if the minimum order qty at purchasing is less than order qty at Replenish Tab then take replenish qty
                sql = "  UPDATE t_replenish SET  QtyToOrder= serial_no  WHERE  order_min < serial_no AND serial_no >QtyToOrder  AND ReplenishType='2' AND  ad_pinstance_id =" + pins.GetAD_PInstance_ID();
                no = DB.ExecuteQuery(sql, null, null);
                //end

                sql = "UPDATE t_replenish SET serial_no = null where ad_pinstance_id =" + pins.GetAD_PInstance_ID();
                no = DB.ExecuteQuery(sql, null, null);
            }
            //end

            // Check (On hand Qty + Qty To Order) Not Greater Than MAXIMUM LEVEL 
            #region   Check (On hand Qty + Qty To Order) Not Greater Than MAXIMUM LEVEL (22 jan 2015)
            sql = "update t_replenish set  QtyToOrder=(Level_Max-QtyOnHand)   WHERE (QtyOnHand+ QtyToOrder)> Level_Max And Level_Max > 0 AND ad_pinstance_id =" + pins.GetAD_PInstance_ID();
            no = DB.ExecuteQuery(sql, null, null);
            //  Consider Order Pack Qty Again  if pack check Box True

            // Commented By Lokesh Chauhan
            if ((_ReplenishmentCreate == "POR" || _ReplenishmentCreate == "POO") & _IsOdrPckQty == "Y")
            //if (_ReplenishmentCreate != "MMM" & _IsOdrPckQty == "Y")
            {
                sql = "UPDATE T_Replenish"
                    + " SET QtyToOrder = QtyToOrder - MOD(QtyToOrder, Order_Pack) "
                    + "WHERE MOD(QtyToOrder, Order_Pack) <> 0"
                    + " AND AD_PInstance_ID=" + pins.GetAD_PInstance_ID();
                no = DB.ExecuteQuery(sql, null, null);
            }

            #endregion

            #region Negative Qty to order not consider  2/4/2015
            //	Delete rows where qty to order negtv(-)
            sql = "DELETE FROM T_Replenish "
                + "WHERE QtyToOrder <= 0"
                + " AND AD_PInstance_ID=" + pins.GetAD_PInstance_ID();
            no = DB.ExecuteQuery(sql, null, null);
            if (no != 0)
            {
                // //log.Fine("Delete No QtyToOrder=" + no);
            }
            #endregion

            //	Source from other warehouse
            if (wh.GetM_WarehouseSource_ID() != 0)
            {
                sql = "UPDATE T_Replenish"
                    + " SET M_WarehouseSource_ID=" + wh.GetM_WarehouseSource_ID()
                    + " WHERE AD_PInstance_ID=" + pins.GetAD_PInstance_ID();
                no = DB.ExecuteQuery(sql, null, null);
                if (no != 0)
                {
                    ////log.Fine("Set Warehouse Source Warehouse=" + no);
                }
            }
            //	Replenishment on Product level overwrites 
            sql = "UPDATE T_Replenish "
                + "SET M_WarehouseSource_ID=(SELECT M_WarehouseSource_ID FROM M_Replenish r "
                    + "WHERE r.M_Product_ID=T_Replenish.M_Product_ID"
                    + " AND r.M_Warehouse_ID=" + _M_Warehouse_ID + " AND r.M_Attributesetinstance_ID = T_Replenish.M_Attributesetinstance_ID)"
                + "WHERE AD_PInstance_ID=" + pins.GetAD_PInstance_ID()
                + " AND EXISTS (SELECT * FROM M_Replenish r "
                    + "WHERE r.M_Product_ID=T_Replenish.M_Product_ID"
                    + " AND r.M_Attributesetinstance_ID=T_Replenish.M_Attributesetinstance_ID"
                    + " AND r.M_Warehouse_ID=" + _M_Warehouse_ID
                    + " AND r.M_WarehouseSource_ID > 0)";
            no = DB.ExecuteQuery(sql, null, null);
            if (no != 0)
            {
                // //log.Fine("Set Product Source Warehouse=" + no);
            }

            //	Check Source Warehouse
            sql = "UPDATE T_Replenish"
                + " SET M_WarehouseSource_ID = NULL "
                + "WHERE M_Warehouse_ID=M_WarehouseSource_ID"
                + " AND AD_PInstance_ID=" + pins.GetAD_PInstance_ID();
            no = DB.ExecuteQuery(sql, null, null);
            if (no != 0)
            {
                //log.Fine("Set same Source Warehouse=" + no);
            }

            ////	Custom Replenishment
            //String className = wh.GetReplenishmentClass();
            //if (className == null || className.Length == 0)
            //{
            //    return;
            //}
            ////	Get Replenishment Class
            //ReplenishInterface custom = null;
            //try
            //{
            //    //Class<?> clazz = Class.forName(className);
            //    Type clazz = Type.GetType(className);
            //    custom = (ReplenishInterface)Activator.CreateInstance(clazz);//.newInstance();
            //}
            //catch (Exception e)
            //{
            //    throw new Exception("No custom Replenishment class "
            //        + className + " - " + e.ToString());
            //}

            //X_T_Replenish[] replenishs = GetReplenish("ReplenishType='9'");
            //for (int i = 0; i < replenishs.Length; i++)
            //{
            //    X_T_Replenish replenish = replenishs[i];
            //    if (replenish.GetReplenishType().Equals(X_T_Replenish.REPLENISHTYPE_Custom))
            //    {
            //        Decimal? qto = null;
            //        try
            //        {
            //            qto = custom.GetQtyToOrder(wh, replenish);
            //        }
            //        catch (Exception e)
            //        {
            //            // log.Log(Level.SEVERE, custom.ToString(), e);
            //        }
            //        if (qto == null)
            //        {
            //            qto = Env.ZERO;
            //        }
            //        replenish.SetQtyToOrder(qto);
            //        replenish.Save();
            //    }
            //}
            //	fillTable
        }

        /// <summary>
        /// Create PO's
        /// </summary>
        private void CreatePO()
        {
            int noOrders = 0;
            String info = "";
            // int _CountED008 = 0;
            int _CountTaxType = 0;
            int _VLTaxType_ID = 0;
            int _VTaxType_ID = 0;
            int _TaxCtgry_ID = 0;
            int _TaxID = 0;
            //Amit
            //  int _M_DiscountSchema_ID = 0;
            //   String _discountCalculation = string.Empty;
            //    decimal valueBasedDiscount = 0;
            //Amit
            String _qry = null;
            // MOrder order = null;
            VAdvantage.Model.MOrder order = null;
            VAdvantage.Model.MBPartner bp = null; //v
            MWarehouse wh = null;
            X_T_Replenish[] replenishs = GetReplenish(_M_WareSource);
            for (int i = 0; i < replenishs.Length; i++)
            {
                X_T_Replenish replenish = replenishs[i];
                if (wh == null || wh.GetM_Warehouse_ID() != replenish.GetM_Warehouse_ID())
                {
                    wh = MWarehouse.Get(_ct, replenish.GetM_Warehouse_ID());
                }
                //
                if (order == null
                    || order.GetC_BPartner_ID() != replenish.GetC_BPartner_ID()
                    || order.GetM_Warehouse_ID() != replenish.GetM_Warehouse_ID())
                {
                    // order = new MOrder(ct, 0, null);
                    order = new VAdvantage.Model.MOrder(_ct, 0, null);
                    order.SetIsSOTrx(false);
                    order.SetC_DocTypeTarget_ID(_C_DocType_ID);
                    order.SetC_DocType_ID(_C_DocType_ID);
                    // MBPartner bp = new MBPartner(ct, replenish.GetC_BPartner_ID(), null);
                    bp = new VAdvantage.Model.MBPartner(_ct, replenish.GetC_BPartner_ID(), null);
                    order.SetBPartner(bp);
                    // Uncomment THIS **************************************************************************
                    //order.SetSalesRep_ID(GetAD_User_ID());
                    order.SetDescription(Msg.GetMsg(_ct, "Replenishment"));
                    //	Set Org/WH
                    order.SetAD_Org_ID(wh.GetAD_Org_ID());
                    order.SetM_Warehouse_ID(wh.GetM_Warehouse_ID());

                    if (!order.Save())
                    {
                        return;
                    }
                    else
                    {
                        if (!createdRecord.Contains(order.GetC_Order_ID()))
                        {
                            createdRecord.Add(order.GetC_Order_ID());
                        }
                    }
                    //log.Fine(order.ToString());
                    noOrders++;
                    info += " - " + order.GetDocumentNo();
                }
                // MOrderLine line = new MOrderLine(order);
                VAdvantage.Model.MOrderLine line = null;
                line = new VAdvantage.Model.MOrderLine(order);
                line.SetM_Product_ID(replenish.GetM_Product_ID());
                //line.SetQty(replenish.GetQtyToOrder());
                int UOM = 0, prdUOM = 0;
                decimal? OrdQty = 0, OrignlQty = 0;
                double Discount = 0;
                prdUOM = Util.GetValueOfInt(DB.ExecuteScalar("SELECT C_UOM_ID  FROM  M_Product    prdct WHERE prdct.isactive='Y' AND prdct.M_Product_ID=" + replenish.GetM_Product_ID()));
                UOM = Util.GetValueOfInt(DB.ExecuteScalar(@"SELECT C_UOM_ID FROM M_Product_PO WHERE IsActive='Y' AND M_Product_ID=" + replenish.GetM_Product_ID() +
                                        " AND C_BPartner_ID = " + replenish.GetC_BPartner_ID()));
                #region Calculate Quantity
                OrdQty = replenish.GetQtyToOrder();
                if (prdUOM != UOM)
                {
                    decimal? Res = Util.GetValueOfDecimal(DB.ExecuteScalar("SELECT trunc(multiplyrate,4) FROM C_UOM_Conversion WHERE C_UOM_ID = " + prdUOM + " AND C_UOM_To_ID = " + UOM + " AND M_Product_ID= " + replenish.GetM_Product_ID(), null, null));
                    if (Res > 0)
                    {
                        OrignlQty = OrdQty;
                        OrdQty = OrdQty * Res;
                        //OrdQty = MUOMConversion.ConvertProductTo(ct, _M_Product_ID, UOM, OrdQty);
                    }
                    else
                    {
                        decimal? res = Util.GetValueOfDecimal(DB.ExecuteScalar("SELECT trunc(multiplyrate,4) FROM C_UOM_Conversion WHERE C_UOM_ID = " + prdUOM + " AND C_UOM_To_ID = " + UOM, null, null));
                        if (res > 0)
                        {
                            OrignlQty = OrdQty;
                            OrdQty = OrdQty * res;
                            // OrdQty = MUOMConversion.Convert(ct, prdUOM, UOM, OrdQty);
                        }
                        else
                        {
                            //msg1.Append(prdUOM + ", ");
                            // return;
                        }
                    }
                }
                else
                {
                    OrignlQty = OrdQty;

                }
                #endregion


                line.SetPrice();
                #region Set TaxID  on PoLine if TaxType Module Downloaded 1/Dec/2014 (vikas)
                _CountTaxType = Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(AD_MODULEINFO_ID) FROM AD_MODULEINFO WHERE PREFIX='VATAX_'"));
                if (_CountTaxType > 0)
                {
                    //Get TaxCategory from Product  & TaxType from VendorLocation
                    _TaxCtgry_ID = Util.GetValueOfInt(DB.ExecuteScalar("SELECT C_TaxCategory_ID  FROM  M_Product prdct WHERE prdct.isactive='Y' AND prdct.M_Product_ID=" + replenish.GetM_Product_ID()));
                    _VLTaxType_ID = Util.GetValueOfInt(DB.ExecuteScalar("SELECT VATAX_TaxType_ID  FROM  C_BPartner_Location bpl WHERE bpl.isactive='Y' AND  bpl.C_BPartner_ID=" + order.GetC_BPartner_ID() + " and bpl.C_BPartner_Location_ID=" + order.GetC_BPartner_Location_ID()));
                    if (_VLTaxType_ID > 0)
                    {
                        _TaxID = Util.GetValueOfInt(DB.ExecuteScalar("SELECT TaxRate.c_tax_id FROM C_TaxCategory Taxctgry INNER JOIN VATAX_TaxCatRate TaxRate ON(Taxctgry.C_TaxCategory_ID=TaxRate.C_TaxCategory_ID)  WHERE Taxctgry.isactive='Y'  AND Taxctgry.C_TaxCategory_ID=" + _TaxCtgry_ID + " AND TaxRate.VATAX_TaxType_ID=" + _VLTaxType_ID));
                        line.SetC_Tax_ID(_TaxID);
                    }
                    else
                    {
                        // Get TaxType From VendorHeader
                        _VTaxType_ID = Util.GetValueOfInt(DB.ExecuteScalar("SELECT VATAX_TaxType_ID  FROM  C_BPartner bp WHERE bp.isactive='Y' AND  bp.C_BPartner_ID=" + order.GetC_BPartner_ID()));
                        if (_VTaxType_ID > 0)
                        {
                            _TaxID = Util.GetValueOfInt(DB.ExecuteScalar("SELECT TaxRate.c_tax_id FROM C_TaxCategory Taxctgry INNER JOIN VATAX_TaxCatRate TaxRate ON(Taxctgry.C_TaxCategory_ID=TaxRate.C_TaxCategory_ID)  WHERE Taxctgry.isactive='Y'  AND Taxctgry.C_TaxCategory_ID=" + _TaxCtgry_ID + " AND TaxRate.VATAX_TaxType_ID=" + _VTaxType_ID));
                            line.SetC_Tax_ID(_TaxID);
                        }
                    }
                    order.GetC_BPartner_Location_ID();

                }
                #endregion

                #region  Set Price  1/Dec/2014
                decimal? ListPrice = 0, Price = 0, UnitPrice = 0;
                // int UOM = 0, prdUOM = 0;
                // prdUOM = Util.GetValueOfInt(DB.ExecuteScalar("SELECT C_UOM_ID  FROM  M_Product    prdct WHERE prdct.isactive='Y' AND prdct.M_Product_ID=" + replenish.GetM_Product_ID()));
                // UOM = Util.GetValueOfInt(DB.ExecuteScalar("SELECT C_UOM_ID  FROM  M_Product_PO prdct WHERE prdct.isactive='Y' AND prdct.M_Product_ID=" + replenish.GetM_Product_ID()));
                // Tuple<String, String> aInfo = null;
                int PriceListVersion = Util.GetValueOfInt(DB.ExecuteScalar("select max(m_pricelist_version_id) from m_pricelist_version where validfrom<=sysdate  and isactive='Y' and m_pricelist_id=" + order.GetM_PriceList_ID()));
                StringBuilder SQL = new StringBuilder();
                StringBuilder SqlUom = new StringBuilder();
                DataSet DsPrice = new DataSet();
                SQL.Append(@"SELECT PriceList , PriceStd , PriceLimit FROM M_ProductPrice WHERE Isactive='Y' AND M_Product_ID = " + replenish.GetM_Product_ID()
                                             + " AND M_PriceList_Version_ID = " + PriceListVersion);

                if (!Env.HasModulePrefix("VAPRC_", out aInfo) && !Env.HasModulePrefix("ED011_", out aInfo))
                {
                    DsPrice = DB.ExecuteDataset(SQL.ToString());
                    if (DsPrice.Tables[0].Rows.Count > 0)
                    {
                        ListPrice = Util.GetValueOfDecimal(DsPrice.Tables[0].Rows[0]["PriceList"]);
                        UnitPrice = Util.GetValueOfDecimal(DsPrice.Tables[0].Rows[0]["PriceStd"]);
                        Price = UnitPrice;
                    }
                }
                if (Env.HasModulePrefix("VAPRC_", out aInfo))
                {
                    SQL.Append(" AND M_AttributeSetInstance_ID = 0");
                    if (Env.HasModulePrefix("ED011_", out aInfo))
                    {
                        SqlUom.Append(SQL);
                        SqlUom.Append(" AND C_UOM_ID=" + UOM);
                        DsPrice = DB.ExecuteDataset(SqlUom.ToString());
                        if (DsPrice.Tables[0].Rows.Count > 0)
                        {
                            ListPrice = Util.GetValueOfDecimal(DsPrice.Tables[0].Rows[0]["PriceList"]);
                            UnitPrice = Util.GetValueOfDecimal(DsPrice.Tables[0].Rows[0]["PriceStd"]);
                            Price = UnitPrice;
                        }
                        else
                        {
                            SqlUom.Clear();
                            SqlUom.Append(SQL);
                            SqlUom.Append(" AND C_UOM_ID=" + prdUOM);
                            DsPrice = DB.ExecuteDataset(SqlUom.ToString());
                            if (DsPrice.Tables[0].Rows.Count > 0)
                            {
                                ListPrice = Util.GetValueOfDecimal(DsPrice.Tables[0].Rows[0]["PriceList"]);
                                UnitPrice = Util.GetValueOfDecimal(DsPrice.Tables[0].Rows[0]["PriceStd"]);
                                Price = UnitPrice;
                                if (prdUOM != UOM)
                                {
                                    //decimal? Res = Util.GetValueOfDecimal(DB.ExecuteScalar("SELECT trunc(multiplyrate,4) FROM C_UOM_Conversion WHERE C_UOM_ID = " + prdUOM + " AND C_UOM_To_ID = " + UOM + " AND M_Product_ID= " + replenish.GetM_Product_ID(), null, null));
                                    decimal? Res = Util.GetValueOfDecimal(DB.ExecuteScalar("SELECT trunc(DivideRate,4) FROM C_UOM_Conversion WHERE C_UOM_ID = " + prdUOM + " AND C_UOM_To_ID = " + UOM + " AND M_Product_ID= " + replenish.GetM_Product_ID(), null, null));
                                    if (Res > 0)
                                    {
                                        Price = UnitPrice * Res;
                                        UnitPrice = UnitPrice * Res;
                                        ListPrice = ListPrice * Res;

                                        //OrdQty = MUOMConversion.ConvertProductTo(ct, _M_Product_ID, UOM, OrdQty);
                                    }
                                    else
                                    {
                                        decimal? res = Util.GetValueOfDecimal(DB.ExecuteScalar("SELECT trunc(DivideRate,4) FROM C_UOM_Conversion WHERE C_UOM_ID = " + prdUOM + " AND C_UOM_To_ID = " + UOM, null, null));
                                        if (res > 0)
                                        {

                                            Price = UnitPrice * res;
                                            UnitPrice = UnitPrice * res;
                                            ListPrice = ListPrice * res;
                                            // OrdQty = MUOMConversion.Convert(ct, prdUOM, UOM, OrdQty);
                                        }

                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        DsPrice = DB.ExecuteDataset(SQL.ToString());
                        if (DsPrice.Tables[0].Rows.Count > 0)
                        {
                            ListPrice = Util.GetValueOfDecimal(DsPrice.Tables[0].Rows[0]["PriceList"]);
                            UnitPrice = Util.GetValueOfDecimal(DsPrice.Tables[0].Rows[0]["PriceStd"]);
                            Price = UnitPrice;
                        }
                    }
                }
                if (!Env.HasModulePrefix("VAPRC_", out aInfo) && Env.HasModulePrefix("ED011_", out aInfo))
                {
                    SqlUom.Append(SQL);
                    SqlUom.Append(" AND C_UOM_ID=" + UOM);
                    DsPrice = DB.ExecuteDataset(SqlUom.ToString());
                    if (DsPrice.Tables[0].Rows.Count > 0)
                    {
                        ListPrice = Util.GetValueOfDecimal(DsPrice.Tables[0].Rows[0]["PriceList"]);
                        UnitPrice = Util.GetValueOfDecimal(DsPrice.Tables[0].Rows[0]["PriceStd"]);
                        Price = UnitPrice;
                    }
                    else
                    {
                        SqlUom.Clear();
                        SqlUom.Append(SQL);
                        SqlUom.Append(" AND C_UOM_ID=" + prdUOM);
                        DsPrice = DB.ExecuteDataset(SqlUom.ToString());
                        if (DsPrice.Tables[0].Rows.Count > 0)
                        {
                            ListPrice = Util.GetValueOfDecimal(DsPrice.Tables[0].Rows[0]["PriceList"]);
                            UnitPrice = Util.GetValueOfDecimal(DsPrice.Tables[0].Rows[0]["PriceStd"]);
                            Price = UnitPrice;
                            if (prdUOM != UOM)
                            {
                                decimal? Res = Util.GetValueOfDecimal(DB.ExecuteScalar("SELECT trunc(DivideRate,4) FROM C_UOM_Conversion WHERE C_UOM_ID = " + prdUOM + " AND C_UOM_To_ID = " + UOM + " AND M_Product_ID= " + replenish.GetM_Product_ID(), null, null));
                                if (Res > 0)
                                {
                                    Price = UnitPrice * Res;
                                    UnitPrice = UnitPrice * Res;
                                    ListPrice = ListPrice * Res;
                                    //OrdQty = MUOMConversion.ConvertProductTo(ct, _M_Product_ID, UOM, OrdQty);
                                }
                                else
                                {
                                    decimal? res = Util.GetValueOfDecimal(DB.ExecuteScalar("SELECT trunc(DivideRate,4) FROM C_UOM_Conversion WHERE C_UOM_ID = " + prdUOM + " AND C_UOM_To_ID = " + UOM, null, null));
                                    if (res > 0)
                                    {
                                        Price = UnitPrice * res;
                                        UnitPrice = UnitPrice * res;
                                        ListPrice = ListPrice * res;
                                        // OrdQty = MUOMConversion.Convert(ct, prdUOM, UOM, OrdQty);
                                    }
                                }
                            }
                        }
                        else
                        {
                            DsPrice = DB.ExecuteDataset(SQL.ToString());
                            if (DsPrice.Tables[0].Rows.Count > 0)
                            {
                                ListPrice = Util.GetValueOfDecimal(DsPrice.Tables[0].Rows[0]["PriceList"]);
                                UnitPrice = Util.GetValueOfDecimal(DsPrice.Tables[0].Rows[0]["PriceStd"]);
                                Price = UnitPrice;
                            }
                        }
                    }
                }
                SqlUom.Clear();
                SQL.Clear();
                #endregion
                if (Env.HasModulePrefix("ED007_", out aInfo))
                {
                    decimal? DiscPercent = Util.GetValueOfDecimal(DB.ExecuteScalar("select discount from c_paymentterm where c_paymentterm_id=" + order.GetC_PaymentTerm_ID()));
                    line.SetED007_DiscountPercent(DiscPercent);
                }
                int precision = MCurrency.GetStdPrecision(_ct, order.GetC_Currency_ID());
                line.SetQty(OrdQty.Value);
                line.SetQtyOrdered(OrignlQty);
                ListPrice = Decimal.Round(ListPrice.Value, precision);
                Price = Decimal.Round(Price.Value, precision);
                UnitPrice = Decimal.Round(UnitPrice.Value, precision);
                line.SetPriceList(ListPrice);
                line.SetPriceEntered(Price);
                line.SetPriceActual(UnitPrice);
                decimal? LineNetAmt = OrdQty.Value * Price.Value;
                line.SetLineNetAmt(Decimal.Round(LineNetAmt.Value, precision));
                line.SetC_UOM_ID(UOM);
                decimal Num = Util.GetValueOfDecimal(ListPrice - UnitPrice);

                Discount = Util.GetValueOfDouble((Num / ListPrice) * 100);
                Discount = Math.Round(Discount, 2);
                line.SetDiscount(Util.GetValueOfDecimal(Discount));
                line.Save();
                if (!line.Save())
                {
                    //
                }
            }
            //_info = "#" + noOrders + info;
            //log.Info(_info);
        }	//	createPO

        ///// <summary>
        ///// Create Requisition
        ///// </summary>
        //private void CreateRequisition()
        //{
        //    int noReqs = 0;
        //    String info = "";
        //    //
        //    //   MRequisition requisition = null;
        //    VAdvantage.Model.MRequisition requisition = null;
        //    MWarehouse wh = null;
        //    X_T_Replenish[] replenishs = GetReplenish(_M_WareSource);
        //    for (int i = 0; i < replenishs.Length; i++)
        //    {
        //        X_T_Replenish replenish = replenishs[i];
        //        if (wh == null || wh.GetM_Warehouse_ID() != replenish.GetM_Warehouse_ID())
        //        {
        //            wh = MWarehouse.Get(_ct, replenish.GetM_Warehouse_ID());
        //        }
        //        //
        //        //old     if (requisition == null
        //        //old       || requisition.GetM_Warehouse_ID() != replenish.GetM_Warehouse_ID())
        //        if (requisition == null   //newchange
        //           || requisition.GetDTD001_MWarehouseSource_ID() != replenish.GetM_WarehouseSource_ID())
        //        {
        //            requisition = new VAdvantage.Model.MRequisition(_ct, 0, null);
        //            requisition.SetAD_User_ID(GetAD_User_ID());
        //            requisition.SetC_DocType_ID(_C_DocType_ID);
        //            requisition.SetDescription(Msg.GetMsg(_ct, "Replenishment"));
        //            //	Set Org/WH
        //            //vikas  SetSourcehouse
        //            int _CountDTD001 = Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(AD_MODULEINFO_ID) FROM AD_MODULEINFO WHERE PREFIX='DTD001_'"));
        //            if (_CountDTD001 > 0)
        //            {
        //                requisition.SetDTD001_MWarehouseSource_ID(replenish.GetM_WarehouseSource_ID());
        //            }
        //            requisition.SetAD_Org_ID(wh.GetAD_Org_ID());
        //            requisition.SetM_Warehouse_ID(wh.GetM_Warehouse_ID());

        //            if (!requisition.Save())
        //            {
        //                return;
        //            }
        //            _DocNo = requisition.GetDocumentNo() + ","; //dtd
        //            //log.Fine(requisition.ToString());
        //            noReqs++;
        //            info += " - " + requisition.GetDocumentNo();
        //        }

        //        //MRequisitionLine line = new MRequisitionLine(requisition);
        //        VAdvantage.Model.MRequisitionLine line = new VAdvantage.Model.MRequisitionLine(requisition);
        //        //ViennaAdvantage.Model.MRequisitionLine line = new ViennaAdvantage.Model.MRequisitionLine(ct , 0 , null);
        //        line.SetM_Requisition_ID(requisition.GetM_Requisition_ID());
        //        line.SetM_Product_ID(replenish.GetM_Product_ID());
        //        line.SetC_BPartner_ID(replenish.GetC_BPartner_ID());
        //        line.SetQty(replenish.GetQtyToOrder());
        //        //    line.SetPrice();
        //        if (!line.Save())
        //        {

        //        }
        //        if (i < replenishs.Length - 1)
        //        {
        //            replenish = replenishs[i + 1];

        //            if (requisition == null   //newchange
        //                      || requisition.GetDTD001_MWarehouseSource_ID() != replenish.GetM_WarehouseSource_ID()) // Set docstatus
        //            {

        //                if (_DocStatus == "DR")
        //                {
        //                    requisition.SetDocStatus(_DocStatus);
        //                }
        //                if (_DocStatus == "IP")
        //                {
        //                    requisition.PrepareIt();
        //                    requisition.SetDocStatus("IP");
        //                }
        //                if (_DocStatus == "CO")
        //                {
        //                    string chk = requisition.CompleteIt();
        //                    if (chk == "CO")
        //                    {
        //                        requisition.SetDocStatus("CO");
        //                    }
        //                }
        //                requisition.Save();
        //            }
        //        }
        //        if (i == replenishs.Length - 1)
        //        {
        //            {

        //                if (_DocStatus == "DR")
        //                {
        //                    requisition.SetDocStatus(_DocStatus);
        //                }
        //                if (_DocStatus == "IP")
        //                {
        //                    requisition.PrepareIt();
        //                    requisition.SetDocStatus("IP");
        //                }
        //                if (_DocStatus == "CO")
        //                {
        //                    string chk = requisition.CompleteIt();
        //                    if (chk == "CO")
        //                    {
        //                        requisition.SetDocStatus("CO");
        //                    }
        //                }
        //                requisition.Save();
        //            }

        //        }
        //    }


        //    //_info = "#" + noReqs + info;
        //    //log.Info(_info);
        //}	//	createRequisition

        ///// <summary>
        ///// Create Inventory Movements
        ///// </summary>
        //private void CreateMovements()
        //{
        //    int noMoves = 0;
        //    String info = "";
        //    //
        //    MClient client = null;
        //    VAdvantage.Model.MMovement move = null;
        //    int M_Warehouse_ID = 0;
        //    int M_WarehouseSource_ID = 0;
        //    MWarehouse whSource = null;
        //    MWarehouse whTarget = null;

        //    string param = "";
        //    if (_M_WareSource != null)
        //    {
        //        param = _M_WareSource;
        //    }
        //    else
        //    {
        //        param = "M_WarehouseSource_ID IS NOT NULL";
        //    }
        //    X_T_Replenish[] replenishs = GetReplenish(param); ;
        //    for (int i = 0; i < replenishs.Length; i++)
        //    {
        //        X_T_Replenish replenish = replenishs[i];
        //        if (whSource == null || whSource.GetM_WarehouseSource_ID() != replenish.GetM_WarehouseSource_ID())
        //        {
        //            whSource = MWarehouse.Get(_ct, replenish.GetM_WarehouseSource_ID());
        //        }
        //        if (whTarget == null || whTarget.GetM_Warehouse_ID() != replenish.GetM_Warehouse_ID())
        //        {
        //            whTarget = MWarehouse.Get(_ct, replenish.GetM_Warehouse_ID());
        //        }
        //        if (client == null || client.GetAD_Client_ID() != whSource.GetAD_Client_ID())
        //        {
        //            client = MClient.Get(_ct, whSource.GetAD_Client_ID());
        //        }
        //        //
        //        if (move == null
        //            || M_WarehouseSource_ID != replenish.GetM_WarehouseSource_ID()
        //            || M_Warehouse_ID != replenish.GetM_Warehouse_ID())
        //        {
        //            M_WarehouseSource_ID = replenish.GetM_WarehouseSource_ID();
        //            M_Warehouse_ID = replenish.GetM_Warehouse_ID();

        //            //if (M_WarehouseSource_ID == 0)
        //            //{
        //            //    M_WarehouseSource_ID = whTarget.GetM_WarehouseSource_ID();
        //            //}

        //            move = new VAdvantage.Model.MMovement(_ct, 0, null);
        //            move.SetC_DocType_ID(_C_DocType_ID);
        //            move.SetDescription(Msg.GetMsg(_ct, "Replenishment")
        //                + ": " + whSource.GetName() + "->" + whTarget.GetName());
        //            //	Set Org
        //            move.SetAD_Org_ID(whSource.GetAD_Org_ID());
        //            move.SetDTD001_MWarehouseSource_ID(M_WarehouseSource_ID);
        //            move.SetMovementDate(DateTime.Now);
        //            move.SetM_Warehouse_ID(M_Warehouse_ID);
        //            if (!move.Save())
        //            {
        //                return;
        //            }
        //            else
        //            {
        //                if (!createdRecord.Contains(move.GetM_Movement_ID()))
        //                {
        //                    createdRecord.Add(move.GetM_Movement_ID());
        //                }
        //            }
        //            //log.Fine(move.ToString());
        //            noMoves++;
        //            info += " - " + move.GetDocumentNo();
        //        }
        //        MProduct product = MProduct.Get(_ct, replenish.GetM_Product_ID());
        //        //	To
        //        int M_LocatorTo_ID = GetLocator_ID(product, whTarget);

        //        //	From: Look-up Storage
        //        MProductCategory pc = MProductCategory.Get(_ct, product.GetM_Product_Category_ID());
        //        String MMPolicy = pc.GetMMPolicy();
        //        if (MMPolicy == null || MMPolicy.Length == 0)
        //        {
        //            MMPolicy = client.GetMMPolicy();
        //        }
        //        //
        //        MStorage[] storages = MStorage.GetWarehouse(_ct,
        //            whSource.GetM_Warehouse_ID(), replenish.GetM_Product_ID(), 0, 0,
        //            true, null,
        //            MClient.MMPOLICY_FiFo.Equals(MMPolicy), null);
        //        if (storages == null || storages.Length == 0)
        //        {
        //            //AddLog("No Inventory in " + whSource.GetName() + " for " + product.GetName());
        //            continue;
        //        }
        //        //
        //        Decimal target = replenish.GetQtyToOrder();
        //        for (int j = 0; j < storages.Length; j++)
        //        {
        //            MStorage storage = storages[j];
        //            //if (storage.GetQtyOnHand().signum() <= 0)
        //            if (Env.Signum(storage.GetQtyOnHand()) <= 0)
        //            {
        //                continue;
        //            }
        //            Decimal moveQty = target;
        //            if (storage.GetQtyOnHand().CompareTo(moveQty) < 0)
        //            {
        //                moveQty = storage.GetQtyOnHand();
        //            }
        //            //
        //            VAdvantage.Model.MMovementLine line = new VAdvantage.Model.MMovementLine(move);
        //            line.SetM_Product_ID(replenish.GetM_Product_ID());
        //            line.SetMovementQty(moveQty);
        //            if (replenish.GetQtyToOrder().CompareTo(moveQty) != 0)
        //            {
        //                line.SetDescription("Total: " + replenish.GetQtyToOrder());
        //            }
        //            line.SetM_Locator_ID(storage.GetM_Locator_ID());		//	from
        //            line.SetM_AttributeSetInstance_ID(storage.GetM_AttributeSetInstance_ID());
        //            line.SetM_LocatorTo_ID(M_LocatorTo_ID);					//	to
        //            line.SetM_AttributeSetInstanceTo_ID(storage.GetM_AttributeSetInstance_ID());
        //            line.Save();
        //            //
        //            //target = target.subtract(moveQty);
        //            target = Decimal.Subtract(target, moveQty);
        //            //if (target.signum() == 0)
        //            if (Env.Signum(target) == 0)
        //            {
        //                break;
        //            }
        //        }
        //        if (Env.Signum(target) != 0)
        //        {
        //            //AddLog("Insufficient Inventory in " + whSource.GetName() + " for " + product.GetName() + " Qty=" + target);
        //        }
        //    }
        //    if (replenishs.Length == 0)
        //    {
        //        //_info = "No Source Warehouse";
        //        //log.Warning(_info);
        //    }
        //    else
        //    {
        //        //_info = "#" + noMoves + info;
        //        //log.Info(_info);
        //    }
        //}	//	createRequisition

        /// <summary>
        /// Get Locator_ID
        /// </summary>
        /// <param name="product"> product </param>
        /// <param name="wh">warehouse</param>
        /// <returns>locator with highest priority</returns>
        private int GetLocator_ID(MProduct product, MWarehouse wh)
        {
            int M_Locator_ID = MProductLocator.GetFirstM_Locator_ID(product, wh.GetM_Warehouse_ID());
            /**	
            MLocator[] locators = MProductLocator.getLocators (product, wh.getM_Warehouse_ID());
            for (int i = 0; i < locators.length; i++)
            {
                MLocator locator = locators[i];
                //	Storage/capacity restrictions come here
                return locator.getM_Locator_ID();
            }
            //	default
            **/
            if (M_Locator_ID == 0)
            {
                M_Locator_ID = wh.GetDefaultM_Locator_ID();
            }
            return M_Locator_ID;
        }	//	getLocator_ID


        /// <summary>
        /// Get Replenish Records
        /// </summary>
        /// <param name="where"></param>
        /// <returns>replenish</returns>
        private X_T_Replenish[] GetReplenish(String where)
        {
            String sql = "SELECT * FROM T_Replenish "
                + "WHERE AD_PInstance_ID=@param AND C_BPartner_ID > 0 ";
            if (where != null && where.Length > 0)
            {
                //  sql += " AND " + where;
            }
            sql += " ORDER BY M_Warehouse_ID, M_WarehouseSource_ID, C_BPartner_ID";
            List<X_T_Replenish> list = new List<X_T_Replenish>();
            SqlParameter[] param = new SqlParameter[1];
            IDataReader idr = null;
            try
            {
                //pstmt = DataBase.prepareStatement (sql, Get_Trx());
                //pstmt.setInt (1, getAD_PInstance_ID());
                param[0] = new SqlParameter("@param", pins.GetAD_PInstance_ID());
                //ResultSet rs = pstmt.executeQuery ();
                idr = DB.ExecuteReader(sql, param, null);
                while (idr.Read())
                {
                    list.Add(new X_T_Replenish(_ct, idr, null));
                }
                idr.Close();
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                    idr = null;
                }
                //log.Log(Level.SEVERE, sql, e);
            }
            X_T_Replenish[] retValue = new X_T_Replenish[list.Count];
            //list.toArray (retValue);
            retValue = list.ToArray();
            return retValue;
        }	//	getReplenish

    }
}