using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

using VAdvantage.Utility;
using System.Text;
using ViennaAdvantage.Model;
using VAdvantage.Model;
using VAdvantage.Logging;
using VAdvantage.DataBase;
using System.IO;
using System.Web.Hosting;

namespace VA011.Models
{
    public class InventoryModel
    {
        Tuple<String, String, String> aInfo = null;

        /// <summary>
        /// Get Product in cart
        /// </summary>
        /// <param name="pageNo">pageNo</param>
        /// <param name="pageSize">pageSize</param>
        /// <param name="ctx">ctx</param>
        /// <returns>product</returns>
        public List<ProdCatInfo> GetProdCats(int pageNo, int pageSize, Ctx ctx)
        {
            List<ProdCatInfo> pInfo = new List<ProdCatInfo>();
            string sql = @"SELECT COUNT(M_Product.M_Product_ID) AS ProdCount ,M_Product_Category.M_Product_Category_ID,M_Product_Category.Name
                            FROM M_Product INNER JOIN M_Product_Category ON M_Product.M_Product_Category_ID = M_Product_Category.M_Product_Category_ID 
                            WHERE M_Product.IsActive ='Y' AND M_Product.IsSummary ='N' AND M_Product_Category.AD_Client_ID = " + ctx.GetAD_Client_ID();
            sql += " GROUP BY M_Product_Category.M_Product_Category_ID,M_Product_Category.Name ORDER BY M_Product_Category.Name";
            DataSet ds = VIS.DBase.DB.ExecuteDatasetPaging(sql, pageNo, pageSize);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    ProdCatInfo prodInfo = new ProdCatInfo();
                    prodInfo.Catname = Util.GetValueOfString(ds.Tables[0].Rows[i]["Name"]);
                    prodInfo.M_ProdCatID = Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_Product_Category_ID"]);
                    prodInfo.ProdCount = Util.GetValueOfInt(ds.Tables[0].Rows[i]["ProdCount"]);
                    pInfo.Add(prodInfo);
                }
            }
            return pInfo;
        }

        /// <summary>
        /// Get org Method
        /// </summary>
        /// <param name="ctx">ctx</param>
        /// <returns>Organizations</returns>
        public List<NameIDClass> GetOrganizations(Ctx ctx)
        {
            List<NameIDClass> pInfo = new List<NameIDClass>();
            string sql = @"SELECT AD_Org_ID, Name FROM AD_Org WHERE IsActive='Y' AND IsSummary='N' AND AD_Client_ID = " + ctx.GetAD_Client_ID();
            DataSet ds = DB.ExecuteDataset(sql);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    NameIDClass org = new NameIDClass();
                    org.Name = Util.GetValueOfString(ds.Tables[0].Rows[i]["Name"]);
                    org.ID = Util.GetValueOfInt(ds.Tables[0].Rows[i]["AD_Org_ID"]);
                    pInfo.Add(org);
                }
            }
            return pInfo;
        }
        /// <summary>
        /// select cart value method
        /// </summary>
        /// <param name="ctx">ctx</param>
        /// <param name="M_Product_ID">M_Product_ID</param>
        /// <returns>Data</returns>
        public int selectGrid(Ctx ctx, int M_Product_ID)
        {
            //var qry = "SELECT M_AttributeSet_ID FROM M_Product WHERE M_Product_ID = " + VIS.Utility.Util.getValueOfInt(cartGrid.records[event.recid - 1]["product_ID"]);
            var qry = "SELECT M_AttributeSet_ID FROM M_Product WHERE M_Product_ID = " + M_Product_ID;
            int mattsetid = Util.GetValueOfInt(DB.ExecuteScalar(qry, null, null));
            return mattsetid;
        }
        /// <summary>
        /// GetOrganizations Method
        /// </summary>
        /// <param name="ctx">ctx</param>
        /// <returns>Organizations in dropdown</returns>
        public List<NameIDClass> GetOrganizations(Ctx ctx, string value, bool fill)
        {
            List<NameIDClass> pInfo = new List<NameIDClass>();
            //JID_0549 added IsCostCenter and IsProfitCenter check 
            string sql = @"SELECT AD_Org_ID, Name FROM AD_Org WHERE AD_Client_ID = " + ctx.GetAD_Client_ID() + " AND IsActive = 'Y' AND IsSummary='N' AND AD_Org_ID != 0 AND IsCostCenter='N' AND IsProfitCenter ='N' ";
            if (value != "")
            {
                sql += " AND UPPER(Name) LIKE UPPER('%" + value + "%') ";
            }
            sql += " ORDER BY Name";
            DataSet ds = DB.ExecuteDataset(sql);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    NameIDClass org = new NameIDClass();
                    if (i == 0)
                    {
                        org.Name = Msg.GetMsg(ctx, "VA011_All");
                        org.ID = 9999;
                        pInfo.Add(org);
                        org = new NameIDClass();
                    }
                    org.Name = Util.GetValueOfString(ds.Tables[0].Rows[i]["Name"]);
                    org.ID = Util.GetValueOfInt(ds.Tables[0].Rows[i]["AD_Org_ID"]);
                    pInfo.Add(org);
                }
            }
            return pInfo;
        }
        /// <summary>
        /// GetPLV Method
        /// </summary>
        /// <param name="ctx">ctx</param>
        /// <param name="value">value</param>
        /// <param name="fill">fill</param>
        /// <returns></returns>
        public List<NameIDClass> GetPLV(Ctx ctx, string value, bool fill)
        {
            List<NameIDClass> pInfo = new List<NameIDClass>();
            // Added Isactive check for header
            string sql = @"SELECT pv.M_PriceList_Version_ID, pv.Name FROM M_PriceList_Version pv INNER JOIN M_PriceList pl
                            ON pv.M_PriceList_ID=pl.M_PriceList_ID
                            WHERE pl.isactive   ='Y'
                            AND pv.isactive     ='Y'
                            AND pv.AD_Client_ID =" + ctx.GetAD_Client_ID();
            if (value != "")
            {
                sql += " AND UPPER(Name) LIKE UPPER('%" + value + "%') ";
            }
            sql += " ORDER BY Name";
            DataSet ds = DB.ExecuteDataset(sql);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    NameIDClass org = new NameIDClass();
                    if (i == 0)
                    {
                        org.Name = Msg.GetMsg(ctx, "VA011_All");
                        org.ID = 9999;
                        pInfo.Add(org);
                        org = new NameIDClass();
                    }

                    org.Name = Util.GetValueOfString(ds.Tables[0].Rows[i]["Name"]);
                    org.ID = Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_PriceList_Version_ID"]);
                    pInfo.Add(org);
                }
            }
            return pInfo;
        }
        /// <summary>
        /// GetOrgWarehouse Method
        /// </summary>
        /// <param name="ctx">ctx</param>
        /// <param name="value">value</param>
        /// <param name="org_IDs">org_IDs</param>
        /// <param name="fill">fill</param>
        /// <returns>OrgWarehouse</returns>
        public List<NameIDClass> GetOrgWarehouse(Ctx ctx, string value, List<int> org_IDs, bool fill)
        {
            var orgString = "";
            if (org_IDs != null)
            {
                if (org_IDs.Count > 0)
                {
                    for (var w = 0; w < org_IDs.Count; w++)
                    {
                        if (orgString.Length > 0)
                        {
                            orgString = orgString + ", " + org_IDs[w];
                        }
                        else
                        {
                            orgString += "0, ";
                            orgString += org_IDs[w];
                        }
                    }
                    //sqlDmd.Append(" AND o.AD_Org_ID IN (0, " + orgString + ")");
                }
            }


            List<NameIDClass> pInfo = new List<NameIDClass>();
            string sql = @"SELECT M_Warehouse_ID, Name FROM M_Warehouse WHERE AD_Client_ID = " + ctx.GetAD_Client_ID() + " AND IsActive = 'Y'";
            if (value != "")
            {
                sql += " AND UPPER(Name) LIKE UPPER('%" + value + "%') ";
            }

            if (orgString.Length > 0)
            {
                sql += " AND AD_Org_ID IN (" + orgString + ")";
            }

            sql += " ORDER BY Name";
            DataSet ds = DB.ExecuteDataset(sql);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    NameIDClass org = new NameIDClass();
                    if (i == 0)
                    {
                        org.Name = Msg.GetMsg(ctx, "VA011_All");
                        org.ID = 9999;
                        pInfo.Add(org);
                        org = new NameIDClass();
                    }

                    org.Name = Util.GetValueOfString(ds.Tables[0].Rows[i]["Name"]);
                    org.ID = Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_Warehouse_ID"]);

                    pInfo.Add(org);
                }
            }
            return pInfo;
        }
        /// <summary>
        /// GetOrgWarehouseAll Method
        /// </summary>
        /// <param name="ctx">ctx</param>
        /// <param name="value">value</param>
        /// <param name="org_IDs">org_IDs</param>
        /// <param name="fill">fill</param>
        /// <returns>OrgWarehouseAll</returns>
        public List<NameIDClass> GetOrgWarehouseAll(Ctx ctx, string value, List<int> org_IDs, bool fill)
        {
            var orgString = "";
            if (org_IDs != null)
            {
                if (org_IDs.Count > 0)
                {
                    for (var w = 0; w < org_IDs.Count; w++)
                    {
                        if (orgString.Length > 0)
                        {
                            orgString = orgString + ", " + org_IDs[w];
                        }
                        else
                        {
                            orgString += "0, ";
                            orgString += org_IDs[w];
                        }
                    }
                    //sqlDmd.Append(" AND o.AD_Org_ID IN (0, " + orgString + ")");
                }
            }


            List<NameIDClass> pInfo = new List<NameIDClass>();
            string sql = @"SELECT M_Warehouse_ID, Name FROM M_Warehouse WHERE AD_Client_ID = " + ctx.GetAD_Client_ID() + " AND IsActive = 'Y'";
            if (value != "")
            {
                sql += " AND UPPER(Name) LIKE UPPER('%" + value + "%') ";
            }

            if (orgString.Length > 0)
            {
                sql += " AND AD_Org_ID IN (" + orgString + ")";
            }

            sql += " ORDER BY Name";
            DataSet ds = DB.ExecuteDataset(sql);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    NameIDClass org = new NameIDClass();
                    org.Name = Util.GetValueOfString(ds.Tables[0].Rows[i]["Name"]);
                    org.ID = Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_Warehouse_ID"]);

                    pInfo.Add(org);
                }
            }
            return pInfo;
        }
        /// <summary>
        /// GetBpartnerAll Method
        /// </summary>
        /// <param name="ctx">ctx</param>
        /// <param name="value">value</param>
        /// <param name="org_IDs">org_IDs</param>
        /// <param name="fill">fill</param>
        /// <returns>BpartnerAll</returns>
        public List<NameIDClass> GetBpartnerAll(Ctx ctx, string value, List<int> org_IDs, bool fill)
        {
            var orgString = "";
            if (org_IDs != null)
            {
                if (org_IDs.Count > 0)
                {
                    for (var w = 0; w < org_IDs.Count; w++)
                    {
                        if (orgString.Length > 0)
                        {
                            orgString = orgString + ", " + org_IDs[w];
                        }
                        else
                        {
                            orgString += "0, ";
                            orgString += org_IDs[w];
                        }
                    }
                    //sqlDmd.Append(" AND o.AD_Org_ID IN (0, " + orgString + ")");
                }
            }


            List<NameIDClass> pInfo = new List<NameIDClass>();
            string sql = @"SELECT M_Warehouse_ID, Name FROM M_Warehouse WHERE AD_Client_ID = " + ctx.GetAD_Client_ID() + " AND IsActive = 'Y'";
            if (value != "")
            {
                sql += " AND UPPER(Name) LIKE UPPER('%" + value + "%') ";
            }

            if (orgString.Length > 0)
            {
                sql += " AND AD_Org_ID IN (" + orgString + ")";
            }

            sql += " ORDER BY Name";
            DataSet ds = DB.ExecuteDataset(sql);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    NameIDClass org = new NameIDClass();
                    org.Name = Util.GetValueOfString(ds.Tables[0].Rows[i]["Name"]);
                    org.ID = Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_Warehouse_ID"]);

                    pInfo.Add(org);
                }
            }
            return pInfo;
        }
        /// <summary>
        /// GetWarehouse Method
        /// </summary>
        /// <param name="ctx">ctx</param>
        /// <param name="value">value</param>
        /// <param name="fill">fill</param>
        /// <returns>Warehouse</returns>
        public List<NameIDClass> GetWarehouse(Ctx ctx, string value, bool fill)
        {
            List<NameIDClass> pInfo = new List<NameIDClass>();
            string sql = @"SELECT M_Warehouse_ID, Name FROM M_Warehouse WHERE AD_Client_ID = " + ctx.GetAD_Client_ID() + " AND IsActive = 'Y'";
            if (value != "")
            {
                sql += " AND UPPER(Name) LIKE UPPER('%" + value + "%') ";
            }
            sql += " ORDER BY Name";
            DataSet ds = DB.ExecuteDataset(sql);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    NameIDClass org = new NameIDClass();
                    if (i == 0)
                    {
                        org.Name = Msg.GetMsg(ctx, "VA011_All");
                        org.ID = 9999;
                        pInfo.Add(org);
                        org = new NameIDClass();
                    }

                    org.Name = Util.GetValueOfString(ds.Tables[0].Rows[i]["Name"]);
                    org.ID = Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_Warehouse_ID"]);

                    pInfo.Add(org);
                }
            }
            return pInfo;
        }
        /// <summary>
        /// GetProductCategories Method
        /// </summary>
        /// <param name="ctx">ctx</param>
        /// <param name="value">value</param>
        /// <param name="fill">fill</param>
        /// <returns>ProductCategories</returns>
        public List<NameIDClass> GetProductCategories(Ctx ctx, string value, bool fill)
        {
            List<NameIDClass> pInfo = new List<NameIDClass>();
            string sql = @"SELECT M_Product_Category_ID, Name FROM M_Product_Category WHERE AD_Client_ID = " + ctx.GetAD_Client_ID() + " AND IsActive = 'Y'";
            if (value != "")
            {
                sql += " AND UPPER(Name) LIKE UPPER('%" + value + "%') ";
            }
            sql += " ORDER BY Name";
            DataSet ds = DB.ExecuteDataset(sql);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    NameIDClass org = new NameIDClass();
                    if (i == 0)
                    {
                        org.Name = Msg.GetMsg(ctx, "VA011_All");
                        org.ID = 9999;
                        pInfo.Add(org);
                        org = new NameIDClass();
                    }

                    org.Name = Util.GetValueOfString(ds.Tables[0].Rows[i]["Name"]);
                    org.ID = Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_Product_Category_ID"]);

                    pInfo.Add(org);
                }
            }
            return pInfo;
        }
        /// <summary>
        /// GetProducts Method
        /// </summary>
        /// <param name="searchText">searchText</param>
        /// <param name="warehouse_IDs">warehouse_IDs</param>
        /// <param name="org_IDs">org_IDs</param>
        /// <param name="plv_IDs">plv_IDs</param>
        /// <param name="supp_IDs">supp_IDs</param>
        /// <param name="prodCat_IDs">prodCat_IDs</param>
        /// <param name="pageNo">pageNo</param>
        /// <param name="pageSize">pageSize</param>
        /// <param name="ct">ct</param>
        /// <returns>Products</returns>
        public List<Products> GetProducts(string searchText, string warehouse_IDs, string org_IDs, string plv_IDs, string supp_IDs, string prodCat_IDs, int pageNo, int pageSize, Ctx ct)
        {
            List<Products> pro = new List<Products>();

            ///************************************************
            StringBuilder sqlDmd = new StringBuilder("(");
            sqlDmd.Append("( SELECT NVL(SUM(ol.QtyReserved),0) AS qtyentered FROM C_Order o INNER JOIN c_orderline ol ON (ol.C_Order_ID = o.C_Order_ID) "
               + " INNER JOIN C_DocType dt ON (o.C_DocTypeTarget_ID = dt.C_DocType_ID) INNER JOIN C_BPartner bp ON (bp.C_BPartner_ID = o.C_BPartner_ID) WHERE o.IsSOTrx = 'Y' AND o.IsReturnTrx = 'N' AND o.DocStatus IN ('CO', 'CL') AND ol.QtyReserved >0 AND ol.M_Product_ID = prd.M_Product_ID )");

            if (Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(*) FROM AD_ModuleInfo WHERE Prefix = 'DTD001_' AND IsActive = 'Y'")) > 0)
            {
                sqlDmd.Append(" + ( SELECT NVL(SUM(rl.DTD001_ReservedQty),0) as qtyentered FROM m_requisitionline rl "
                    + " INNER JOIN M_Requisition r ON (r.M_Requisition_ID = rl.M_Requisition_ID) INNER JOIN C_DocType dt ON (r.C_DocType_ID = dt.C_DocType_ID) INNER JOIN M_Warehouse w "
                    + " ON (r.M_Warehouse_ID = w.M_Warehouse_ID) INNER JOIN m_product prd1 ON (prd1.M_Product_ID = rl.M_Product_ID) WHERE r.DocStatus IN ('CO', 'CL') AND rl.DTD001_ReservedQty > 0 AND r.IsActive = 'Y' AND rl.M_Product_ID = prd.M_Product_ID ) ");
            }

            if (Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(*) FROM AD_ModuleInfo WHERE Prefix = 'VAMFG_' AND IsActive = 'Y'")) > 0)
            {
                sqlDmd.Append(" + ( SELECT NVL(SUM(wo.vamfg_qtyentered),0) AS qtyentered FROM vamfg_M_workorder wo "
                    + " INNER JOIN c_doctype dt ON (dt.C_DocType_ID = wo.C_DocType_ID) LEFT OUTER JOIN C_BPartner bp ON (bp.C_BPartner_ID = wo.C_BPartner_ID) INNER JOIN m_product p "
                    + " ON (p.M_Product_ID = wo.M_Product_ID) WHERE wo.DocStatus IN ('CO', 'CL') AND wo.IsActive   = 'Y' AND wo.M_Product_ID = prd.M_Product_ID) ");
            }

            sqlDmd.Append(" ) AS QtyDemanded,");

            //**********************************//

            string minLevelSQL = "";
            if (warehouse_IDs.Length > 0)
            {
                minLevelSQL = "(SELECT NVL(SUM(LEVEL_MIN),0) FROM  M_Replenish WHERE M_Product_ID = prd.M_Product_ID AND M_Warehouse_ID IN (" + warehouse_IDs + ")) as MinLevel ";
            }
            else
            {
                minLevelSQL = "(SELECT NVL(SUM(LEVEL_MIN),0) FROM  M_Replenish WHERE M_Product_ID = prd.M_Product_ID) as MinLevel ";
            }

            StringBuilder sbSql = new StringBuilder(@"SELECT M_Product_ID,UPC,C_UOM_ID, 0 AS QtyAvailable, UOM, Value, Name, 0 AS QTYENTERED, 0 AS QtyOnHand,  "
          + " 0 AS QtyReserved,0 AS QtyOrdered,"
          + " (SELECT NVL(SUM(lc.TargetQty),0) FROM M_InOutLineConfirm lc"
          + " INNER JOIN M_InOutConfirm ioc ON (ioc.M_InOutConfirm_ID = lc.M_InOutConfirm_ID) INNER JOIN M_InOutLine iol"
          + " ON (iol.M_InOutline_ID = lc.M_InOutLine_ID) INNER JOIN M_InOut io  ON (iol.M_Inout_ID = io.M_InOut_ID) LEFT JOIN M_Storage s1"
          + " ON iol.M_Product_ID = s1.M_Product_ID LEFT OUTER JOIN M_Locator l1 ON s1.M_Locator_ID=l1.M_Locator_ID LEFT OUTER JOIN M_Warehouse w1"
          + " ON l1.M_Warehouse_ID  = w1.M_Warehouse_ID WHERE ioc.DocStatus NOT IN ('CO', 'CL') AND iol.M_Product_ID = prd.M_Product_ID"
          + " AND io.IsSOTrx = 'N' AND iol.M_Locator_ID  IN  (SELECT loc.M_Locator_ID FROM M_Locator loc WHERE loc.M_Warehouse_ID = "
          + " w1.M_Warehouse_ID )) AS QtyUnconfirmed,");
            sbSql.Append(sqlDmd.ToString()).Append(minLevelSQL);
            sbSql.Append(" FROM (SELECT DISTINCT p.M_Product_ID, p.C_UOM_ID, p.UPC, 0 AS QtyAvailable, "
          + " um.Name as UOM, p.Value, p.Name, 0 AS QTYENTERED, "
          // + " bomPriceListUom(p.M_Product_ID, pr.M_PriceList_Version_ID,pr.M_AttriButeSetInstance_ID,pr.C_UOM_ID) AS PriceList, "
          // + " bomPriceStdUom(p.M_Product_ID, pr.M_PriceList_Version_ID,pr.M_AttriButeSetInstance_ID,pr.C_UOM_ID) AS PriceStd,"
          + " 0 AS QtyOnHand, 0 AS QtyReserved,");
            // + " (SELECT NVL(SUM(lc.TargetQty),0) FROM M_InOutLineConfirm lc INNER JOIN M_InOutConfirm ioc ON (ioc.M_InOutConfirm_ID = lc.M_InOutConfirm_ID)  INNER JOIN M_InOutLine iol "
            // + " ON (iol.M_InOutline_ID = lc.M_InOutLine_ID) INNER JOIN M_InOut io ON (iol.M_Inout_ID   = io.M_InOut_ID) WHERE ioc.DocStatus NOT IN ('CO', 'CL')  AND iol.M_Product_ID = p.M_Product_ID"
            //+ " AND io.IsSOTrx = 'N'  AND iol.M_Locator_ID IN ( SELECT loc.M_Locator_ID FROM M_Locator loc WHERE M_Warehouse_ID = w.M_Warehouse_ID)) AS QtyUnconfirmed, ");
            // sbSql.Append(sqlDmd.ToString());
            //+ " ((SELECT NVL(SUM(ol.QtyReserved),0) AS QtyEntered FROM C_Order o INNER JOIN c_orderline ol ON (ol.C_Order_ID = o.C_Order_ID) INNER JOIN C_DocType dt  ON (o.C_DocTypeTarget_ID = dt.C_DocType_ID)"
            //+ " INNER JOIN C_BPartner bp ON (bp.C_BPartner_ID = o.C_BPartner_ID) WHERE o.IsSOTrx = 'Y' AND ol.M_Product_ID  = p.M_Product_ID AND o.IsReturnTrx = 'N' AND o.DocStatus IN ('CO', 'CL') "
            //+ " AND ol.QtyReserved   > 0) "
            //+ " + (SELECT NVL(SUM(rl.DTD001_ReservedQty),0) AS QtyEntered FROM m_requisitionline rl INNER JOIN M_Requisition r ON (r.M_Requisition_ID = rl.M_Requisition_ID) INNER JOIN C_DocType dt "
            //+ " ON (r.C_DocType_ID = dt.C_DocType_ID) INNER JOIN M_Warehouse w ON (r.M_Warehouse_ID = w.M_Warehouse_ID) INNER JOIN m_product p ON (p.M_Product_ID = rl.M_Product_ID) "
            //+ " WHERE r.DocStatus IN ('CO', 'CL') AND rl.DTD001_ReservedQty > 0 AND r.IsActive = 'Y' AND rl.M_Product_ID  = p.M_Product_ID) ) AS QtyDemanded, "
            sbSql.Append(" 0 AS QtyOrdered");
            //   + "(SELECT NVL(SUM(LEVEL_MIN),0) FROM  M_Replenish WHERE M_Product_ID = p.M_Product_ID AND M_Warehouse_ID IN (" + warehouse_IDs + ")) as Level_Min "
            // + " bomPriceStdUom(p.M_Product_ID, pr.M_PriceList_Version_ID,pr.M_AttriButeSetInstance_ID,pr.C_UOM_ID)-bomPriceLimitUom(p.M_Product_ID, pr.M_PriceList_Version_ID,pr.M_AttriButeSetInstance_ID,pr.C_UOM_ID) AS Margin, "
            // + " bomPriceLimitUom(p.M_Product_ID, pr.M_PriceList_Version_ID,pr.M_AttriButeSetInstance_ID,pr.C_UOM_ID) AS PriceLimit,  "
            sbSql.Append(" FROM M_Product p INNER JOIN C_UOM um ON (um.C_UOM_ID=p.C_UOM_ID) LEFT OUTER JOIN M_ProductPrice pr ON (p.M_Product_ID=pr.M_Product_ID AND pr.IsActive   ='Y') "
            + " LEFT OUTER JOIN M_PriceList_Version plv ON (pr.M_PriceList_Version_ID=plv.M_PriceList_Version_ID) "
            + " LEFT OUTER JOIN M_Product_PO pu ON (pu.M_Product_ID = p.M_Product_ID) "
            + " LEFT OUTER JOIN M_AttributeSet pa ON (p.M_AttributeSet_ID=pa.M_AttributeSet_ID) LEFT OUTER JOIN M_manufacturer mr ON (p.M_Product_ID=mr.M_Product_ID)  "
            + " LEFT OUTER JOIN M_ProductAttributes patr ON (p.M_Product_ID=patr.M_Product_ID) left join M_Storage s ON p.M_Product_ID = s.M_Product_ID  LEFT OUTER JOIN M_Locator l "
            //+ " LEFT OUTER JOIN M_Replenish rep ON (rep.M_Product_ID = )"
            + " ON s.M_Locator_ID=l.M_Locator_ID  LEFT OUTER JOIN M_Warehouse w ON l.M_Warehouse_ID = w.M_Warehouse_ID WHERE p.AD_Client_ID = " + ct.GetAD_Client_ID() + " AND p.IsActive='Y' AND p.IsSummary ='N' ");

            StringBuilder sbGroup = new StringBuilder("GROUP BY M_Product_ID,C_UOM_ID, UOM, Value, Name,UPC");
            StringBuilder sbWhere = new StringBuilder();
            if (org_IDs.Length > 0)
            {
                sbWhere.Append(" AND w.AD_Org_ID IN (" + org_IDs + ")");
                //sbGroup.Append(",AD_Org_ID");
            }

            if (warehouse_IDs.Length > 0)
            {
                sbWhere.Append(" AND w.M_Warehouse_ID IN (" + warehouse_IDs + ")");
                //sbGroup.Append(",M_Warehouse_ID");
            }

            if (plv_IDs.Length > 0)
            {
                sbWhere.Append(" AND pr.M_PriceList_Version_ID IN (" + plv_IDs + ")");
                //sbGroup.Append(",M_PriceList_Version_ID");
            }

            if (supp_IDs.Length > 0)
            {
                sbWhere.Append(" AND pu.C_BPartner_ID IN (" + supp_IDs + ")");
                //sbGroup.Append(",C_BPartner_ID");
            }

            if (prodCat_IDs.Length > 0)
            {
                sbWhere.Append(" AND p.M_Product_Category_ID IN (" + prodCat_IDs + ")");
                //sbGroup.Append(",M_Product_Category_ID");
            }
            if (searchText.Length > 0)
            {
                // JID_1296 Should be able search product by using Searchkey and UPC And Name
                sbWhere.Append(" AND UPPER(p.Name) LIKE UPPER('%" + searchText + "%') OR UPPER(p.Value) LIKE UPPER('%" + searchText + "%') OR UPPER(p.UPC) LIKE UPPER('%" + searchText + "%')");
            }

            sbSql.Append(sbWhere.ToString());
            sbSql.Append(" Order by p.name  ) prd ");
            sbSql.Append(sbGroup.ToString());

            DataSet dsPro = null;
            try
            {
                dsPro = VIS.DBase.DB.ExecuteDatasetPaging(sbSql.ToString(), pageNo, pageSize);
                if (dsPro != null)
                {
                    if (dsPro.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < dsPro.Tables[0].Rows.Count; i++)
                        {
                            sbSql.Clear();
                            // changes done to get storage related quantities right, Previously it multiplies qty with number of locators in warehouse.- Done by mohit - asked by mukesh sir. ticket from client- Date : 6-June-2018
                            sbSql.Append("SELECT SUM (QtyAvailable) AS QtyAvailable, SUM(QtyOnHand) AS QtyOnHand, SUM(QtyReserved) AS QtyReserved, SUM(QtyOrdered) AS QtyOrdered FROM ("
                            + "SELECT DISTINCT bomQtyAvailableAttr(p.M_Product_ID,s.M_AttributeSetInstance_ID,w.M_Warehouse_ID,0) AS QtyAvailable,"
                            + " bomQtyOnHandAttr(p.M_Product_ID,s.M_AttributeSetInstance_ID,w.M_Warehouse_ID,0) AS QtyOnHand,"
                            + " bomQtyReservedAttr(p.M_Product_ID,s.M_AttributeSetInstance_ID,w.M_Warehouse_ID,0) AS QtyReserved,"
                            + " bomQtyOrderedAttr(p.M_Product_ID,s.M_AttributeSetInstance_ID,w.M_Warehouse_ID,0) AS QtyOrdered,l.M_Warehouse_ID,s.M_AttributeSetInstance_ID");
                            sbSql.Append(" FROM M_Product p left join M_Storage s ON p.M_Product_ID = s.M_Product_ID INNER JOIN M_Locator l "
                            + " ON s.M_Locator_ID=l.M_Locator_ID  INNER JOIN M_Warehouse w ON l.M_Warehouse_ID = w.M_Warehouse_ID WHERE p.AD_Client_ID = " + ct.GetAD_Client_ID() +
                            " AND p.IsActive='Y' AND p.IsSummary ='N' AND p.M_Product_ID = " + Util.GetValueOfInt(dsPro.Tables[0].Rows[i]["M_Product_ID"]));
                            //sbSql.Append(sbWhere.ToString());
                            if (org_IDs.Length > 0)
                            {
                                sbSql.Append(" AND w.AD_Org_ID IN (" + org_IDs + ")");
                                //sbGroup.Append(",AD_Org_ID");
                            }
                            if (warehouse_IDs.Length > 0)
                            {
                                sbSql.Append(" AND w.M_Warehouse_ID IN (" + warehouse_IDs + ")");
                                //sbGroup.Append(",M_Warehouse_ID");
                            }
                            sbSql.Append(") t");
                            DataSet dsQty = DB.ExecuteDataset(sbSql.ToString());
                            if (dsQty != null)
                            {
                                if (dsQty.Tables[0].Rows.Count > 0)
                                {
                                    Products _prod = new Products();
                                    _prod.M_Product_ID = Util.GetValueOfInt(dsPro.Tables[0].Rows[i]["M_Product_ID"]);
                                    _prod.Value = Util.GetValueOfString(dsPro.Tables[0].Rows[i]["Value"]);
                                    _prod.Name = Util.GetValueOfString(dsPro.Tables[0].Rows[i]["Name"]);
                                    _prod.UPC = Util.GetValueOfString(dsPro.Tables[0].Rows[i]["UPC"]);
                                    _prod.QtyOnHand = Util.GetValueOfDecimal(dsQty.Tables[0].Rows[0]["QtyOnHand"]);
                                    _prod.C_UOM_ID = Util.GetValueOfInt(dsPro.Tables[0].Rows[i]["C_UOM_ID"]);
                                    _prod.UOM = Util.GetValueOfString(dsPro.Tables[0].Rows[i]["UOM"]);
                                    _prod.Reserved = Util.GetValueOfDecimal(dsQty.Tables[0].Rows[0]["QtyReserved"]);
                                    _prod.QtyAvailable = Util.GetValueOfDecimal(dsQty.Tables[0].Rows[0]["QtyAvailable"]);
                                    _prod.UnConfirmed = Util.GetValueOfDecimal(dsPro.Tables[0].Rows[i]["QtyUnconfirmed"]); ;
                                    _prod.Ordered = Util.GetValueOfDecimal(dsQty.Tables[0].Rows[0]["QtyOrdered"]);
                                    _prod.Demanded = Util.GetValueOfDecimal(dsPro.Tables[0].Rows[i]["QtyDemanded"]);
                                    _prod.MinLevel = Util.GetValueOfDecimal(dsPro.Tables[0].Rows[i]["MinLevel"]);
                                    _prod.TillReorder = 0;
                                    _prod.QtyToReplenish = 0;
                                    pro.Add(_prod);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }

            return pro;
        }
        /// <summary>
        /// GeProductCount Method
        /// </summary>
        /// <param name="searchText">searchText</param>
        /// <param name="warehouse_IDs">warehouse_IDs</param>
        /// <param name="org_IDs">org_IDs</param>
        /// <param name="plv_IDs">plv_IDs</param>
        /// <param name="supp_IDs">supp_IDs</param>
        /// <param name="prodCat_IDs">prodCat_IDs</param>
        /// <param name="ct">ct</param>
        /// <returns>Product Count</returns>
        public int GeProductCount(string searchText, string warehouse_IDs, string org_IDs, string plv_IDs, string supp_IDs, string prodCat_IDs, Ctx ct)
        {
            int count = 0;

            StringBuilder sbSql = new StringBuilder(@"SELECT COUNT(*) "
            //+ " SUM(QTYENTERED) AS QTYENTERED, SUM(QtyOnHand) AS QtyOnHand," 
            //+ "SUM(QtyReserved) AS QtyReserved, SUM(QtyAvailable) AS QtyAvailable, SUM(QtyUnconfirmed) AS QtyUnconfirmed,SUM(QtyOrdered) AS QtyOrdered 
            + " FROM (SELECT DISTINCT p.M_Product_ID, p.C_UOM_ID, "
          //bomQtyAvailable(p.M_Product_ID,w.M_Warehouse_ID,0) AS QtyAvailable,"
          + " um.Name as UOM, p.Value, p.Name, 0 AS QTYENTERED "
          // + " bomPriceListUom(p.M_Product_ID, pr.M_PriceList_Version_ID,pr.M_AttriButeSetInstance_ID,pr.C_UOM_ID) AS PriceList, "
          // + " bomPriceStdUom(p.M_Product_ID, pr.M_PriceList_Version_ID,pr.M_AttriButeSetInstance_ID,pr.C_UOM_ID) AS PriceStd,"
          //+ " bomQtyOnHand(p.M_Product_ID,w.M_Warehouse_ID,0) AS QtyOnHand, bomQtyReserved(p.M_Product_ID,w.M_Warehouse_ID,0)  AS QtyReserved,"
          //+ " (SELECT NVL(SUM(lc.TargetQty),0) FROM M_InOutLineConfirm lc INNER JOIN M_InOutConfirm ioc ON (ioc.M_InOutConfirm_ID = lc.M_InOutConfirm_ID)  INNER JOIN M_InOutLine iol "
          //+ " ON (iol.M_InOutline_ID = lc.M_InOutLine_ID) INNER JOIN M_InOut io ON (iol.M_Inout_ID   = io.M_InOut_ID) WHERE ioc.DocStatus NOT IN ('CO', 'CL')  AND iol.M_Product_ID = p.M_Product_ID"
          //+ " AND io.IsSOTrx = 'N'  AND iol.M_Locator_ID IN ( SELECT loc.M_Locator_ID FROM M_Locator loc WHERE M_Warehouse_ID = w.M_Warehouse_ID)) AS QtyUnconfirmed "
          //+ " bomQtyOrdered(p.M_Product_ID,w.M_Warehouse_ID,0) AS QtyOrdered"
          // + " bomPriceStdUom(p.M_Product_ID, pr.M_PriceList_Version_ID,pr.M_AttriButeSetInstance_ID,pr.C_UOM_ID)-bomPriceLimitUom(p.M_Product_ID, pr.M_PriceList_Version_ID,pr.M_AttriButeSetInstance_ID,pr.C_UOM_ID) AS Margin, "
          // + " bomPriceLimitUom(p.M_Product_ID, pr.M_PriceList_Version_ID,pr.M_AttriButeSetInstance_ID,pr.C_UOM_ID) AS PriceLimit,  "
          + " FROM M_Product p INNER JOIN C_UOM um ON (um.C_UOM_ID=p.C_UOM_ID) LEFT OUTER JOIN M_ProductPrice pr ON (p.M_Product_ID=pr.M_Product_ID AND pr.IsActive   ='Y') "
          + " LEFT OUTER JOIN M_PriceList_Version plv ON (pr.M_PriceList_Version_ID=plv.M_PriceList_Version_ID) "
          + " LEFT OUTER JOIN M_Product_PO pu ON (pu.M_Product_ID = p.M_Product_ID) "
          + " LEFT OUTER JOIN M_AttributeSet pa ON (p.M_AttributeSet_ID=pa.M_AttributeSet_ID) LEFT OUTER JOIN M_manufacturer mr ON (p.M_Product_ID=mr.M_Product_ID)  "
          + " LEFT OUTER JOIN M_ProductAttributes patr ON (p.M_Product_ID=patr.M_Product_ID) LEFT OUTER JOIN M_Storage s ON p.M_Product_ID = s.M_Product_ID  LEFT OUTER JOIN M_Locator l "
          + " ON s.M_Locator_ID=l.M_Locator_ID  LEFT OUTER JOIN M_Warehouse w ON l.M_Warehouse_ID = w.M_Warehouse_ID WHERE p.AD_Client_ID = " + ct.GetAD_Client_ID() + " AND p.IsActive='Y' AND p.IsSummary ='N' ");

            StringBuilder sbGroup = new StringBuilder("GROUP BY M_Product_ID,C_UOM_ID, UOM, Value, Name)");

            if (org_IDs.Length > 0)
            {
                sbSql.Append(" AND w.AD_Org_ID IN (" + org_IDs + ")");
                //sbGroup.Append(",AD_Org_ID");
            }

            if (warehouse_IDs.Length > 0)
            {
                sbSql.Append(" AND w.M_Warehouse_ID IN (" + warehouse_IDs + ")");
                //sbGroup.Append(",M_Warehouse_ID");
            }

            if (plv_IDs.Length > 0)
            {
                sbSql.Append(" AND pr.M_PriceList_Version_ID IN (" + plv_IDs + ")");
                //sbGroup.Append(",M_PriceList_Version_ID");
            }

            if (supp_IDs.Length > 0)
            {
                sbSql.Append(" AND pu.C_BPartner_ID IN (" + supp_IDs + ")");
                //sbGroup.Append(",C_BPartner_ID");
            }

            if (prodCat_IDs.Length > 0)
            {
                sbSql.Append(" AND p.M_Product_Category_ID IN (" + prodCat_IDs + ")");
                //sbGroup.Append(",C_BPartner_ID");
            }

            if (searchText.Length > 0)
            {
                sbSql.Append(" AND UPPER(p.Name) LIKE UPPER('%" + searchText + "%') ");
            }

            sbSql.Append(" ORDER BY p.Value DESC) prd ");
            //  sbSql.Append(sbGroup.ToString());
            try
            {
                count = Util.GetValueOfInt(DB.ExecuteScalar(sbSql.ToString(), null, null));
            }
            catch (Exception ex)
            {

            }

            return count;
        }
        /// <summary>
        /// GeReplenishment Method
        /// </summary>
        /// <param name="columns">columns</param>
        /// <param name="whIDs">whIDs</param>
        /// <param name="ct">ct</param>
        /// <returns>Replenishment</returns>
        public List<Decimal?> GeReplenishment(List<Int32> columns, List<int> whIDs, Ctx ct)
        {
            Decimal? QtyOnHand = 0, QtyReserved = 0, QtyOrdered = 0, DTD_QtyReserved = 0, DTD_SourceReserve = 0, QtyToOrder = 0;
            Decimal? levelMin = 0, levelMax = 0, OrderMin = 0, orderPack = 0;
            int ReplenishType = 0;
            DataSet ds = null;
            List<Decimal?> OrderedQty = new List<Decimal?>();
            StringBuilder sb = new StringBuilder("");

            if (whIDs == null)
            {
                whIDs = new List<int>();
                sb.Clear();
                sb.Append("SELECT M_Warehouse_ID FROM M_Warehouse WHERE AD_Client_ID = " + ct.GetAD_Client_ID() + " AND IsActive = 'Y'");
                IDataReader idr = null;
                try
                {
                    idr = DB.ExecuteReader(sb.ToString(), null, null);
                    while (idr.Read())
                    {
                        whIDs.Add(Util.GetValueOfInt(idr[0]));
                    }
                    if (idr != null)
                    {
                        idr.Close();
                        idr = null;
                    }
                }
                catch (Exception ex)
                {
                    if (idr != null)
                    {
                        idr.Close();
                        idr = null;
                    }
                }
            }
            else if (whIDs.Count <= 0)
            {
                sb.Clear();
                sb.Append("SELECT M_Warehouse_ID FROM M_Warehouse WHERE AD_Client_ID = " + ct.GetAD_Client_ID() + " AND IsActive = 'Y'");
                IDataReader idr = null;
                try
                {
                    idr = DB.ExecuteReader(sb.ToString(), null, null);
                    while (idr.Read())
                    {
                        whIDs.Add(Util.GetValueOfInt(idr[0]));
                    }
                    if (idr != null)
                    {
                        idr.Close();
                        idr = null;
                    }
                }
                catch (Exception ex)
                {
                    if (idr != null)
                    {
                        idr.Close();
                        idr = null;
                    }
                }
            }

            for (int i = 0; i < columns.Count; i++)
            {
                ReplenishType = 0;
                levelMin = 0;
                levelMax = 0;

                Decimal? whOrderQty = 0;
                bool found = false;

                if (whIDs == null)
                {
                    break;
                }
                string sql = "UPDATE M_Product_PO"
                    + " SET Order_Pack = 1 "
                    + "WHERE Order_Pack IS NULL OR Order_Pack < 1";
                int no = DB.ExecuteQuery(sql, null, null);

                sql = "UPDATE M_Product_PO"
                    + " SET Order_Min = 1 "
                    + "WHERE Order_Min IS NULL OR Order_Min < 1";
                no = DB.ExecuteQuery(sql, null, null);

                sql = "UPDATE M_Replenish"
                    + " SET DTD001_MinOrderQty = 1 "
                    + "WHERE DTD001_MinOrderQty IS NULL OR DTD001_MinOrderQty < 1";
                no = DB.ExecuteQuery(sql, null, null);

                sql = "UPDATE M_Replenish"
                       + " SET DTD001_OrderPackQty = 1 "
                       + "WHERE DTD001_OrderPackQty IS NULL OR DTD001_OrderPackQty < 1";
                no = DB.ExecuteQuery(sql, null, null);

                for (int j = 0; j < whIDs.Count; j++)
                {
                    sb.Clear();
                    sb.Append("SELECT r.ReplenishType, r.Level_Min, r.Level_Max, r.DTD001_OrderPackQty, r.DTD001_MinOrderQty FROM M_Replenish r WHERE (r.ReplenishType = '1' OR r.ReplenishType ='2')  AND r.IsActive='Y'"
                           + " AND r.M_Product_ID = " + columns[i] + " AND r.M_Warehouse_ID = " + whIDs[j]);
                    ds = new DataSet();
                    ds = DB.ExecuteDataset(sb.ToString(), null, null);
                    if (ds != null && ds.Tables[0].Rows.Count > 0)
                    {
                        found = true;
                        ReplenishType = Util.GetValueOfInt(ds.Tables[0].Rows[0]["ReplenishType"]);
                        levelMin = Util.GetValueOfDecimal(ds.Tables[0].Rows[0]["Level_Min"]);
                        levelMax = Util.GetValueOfDecimal(ds.Tables[0].Rows[0]["Level_Max"]);
                        OrderMin = Util.GetValueOfDecimal(ds.Tables[0].Rows[0]["DTD001_MinOrderQty"]);
                        orderPack = Util.GetValueOfDecimal(ds.Tables[0].Rows[0]["DTD001_OrderPackQty"]);
                    }
                    else
                    {
                        //if(whOrderQty == -999999999)
                        //{
                        //    continue;
                        //}
                        //whOrderQty = -999999999;
                        // OrderedQty.Add(-999999999);
                        continue;
                    }
                    Tuple<String, String, String> mInfo;
                    if (Env.HasModulePrefix("DTD001_", out mInfo))
                    {
                        sb.Clear();
                        sb.Append("SELECT COALESCE(SUM(QtyOnHand),0) AS QtyOnHand,COALESCE(SUM(QtyReserved),0) AS QtyReserved,COALESCE(SUM(QtyOrdered),0) AS QtyOrdered,COALESCE(SUM(DTD001_QtyReserved),0) AS DTD001_QtyReserved, "
                            + "COALESCE(SUM(DTD001_SourceReserve),0) AS DTD001_SourceReserve FROM M_Storage s, M_Locator l WHERE l.M_Locator_ID=s.M_Locator_ID AND s.M_Product_ID = " + columns[i] + " AND l.M_Warehouse_ID = " + whIDs[j]);
                        ds = new DataSet();
                        ds = DB.ExecuteDataset(sb.ToString(), null, null);
                        if (ds != null && ds.Tables[0].Rows.Count > 0)
                        {
                            QtyOnHand = Util.GetValueOfDecimal(ds.Tables[0].Rows[0]["QtyOnHand"]);
                            QtyReserved = Util.GetValueOfDecimal(ds.Tables[0].Rows[0]["QtyReserved"]);
                            QtyOrdered = Util.GetValueOfDecimal(ds.Tables[0].Rows[0]["QtyOrdered"]);
                            DTD_QtyReserved = Util.GetValueOfDecimal(ds.Tables[0].Rows[0]["DTD001_QtyReserved"]);
                            DTD_SourceReserve = Util.GetValueOfDecimal(ds.Tables[0].Rows[0]["DTD001_SourceReserve"]);
                        }

                        if (ReplenishType == 1)
                        {
                            QtyToOrder = levelMin - QtyOnHand + QtyReserved + DTD_SourceReserve - QtyOrdered - DTD_QtyReserved;

                            if (QtyToOrder < 0)
                            {
                                QtyToOrder = 0;
                                continue;
                            }

                            //if (OrderMin > 0 && QtyToOrder < OrderMin)
                            //{
                            //    QtyToOrder = OrderMin;
                            //}

                            if (OrderMin > 0)
                            {
                                QtyToOrder = OrderMin + QtyToOrder;
                            }

                            if ((QtyOnHand + QtyToOrder) > levelMax && levelMax > 0)
                            {
                                QtyToOrder = (levelMax - QtyOnHand);
                            }

                            object qtyPack = (DB.ExecuteScalar("SELECT  " + QtyToOrder + " - MOD(" + QtyToOrder + ", " + orderPack + ") + " + orderPack + " FROM DUAL "
                            + " WHERE MOD( " + QtyToOrder + ", " + orderPack + ") <> 0"));
                            if (qtyPack != null)
                            {
                                QtyToOrder = Util.GetValueOfDecimal(qtyPack);
                            }
                            qtyPack = (DB.ExecuteScalar("SELECT  " + QtyToOrder + " - MOD(" + QtyToOrder + ", " + orderPack + ") FROM DUAL "
                            + " WHERE MOD( " + QtyToOrder + ", " + orderPack + ") <> 0"));
                            if (qtyPack != null)
                            {
                                QtyToOrder = Util.GetValueOfDecimal(qtyPack);
                            }
                        }
                        else if (ReplenishType == 2)
                        {
                            QtyToOrder = levelMax - QtyOnHand + QtyReserved + DTD_SourceReserve - QtyOrdered - DTD_QtyReserved;

                            if (QtyToOrder < 0)
                            {
                                QtyToOrder = 0;
                                continue;
                            }

                            if (OrderMin > 0 && QtyToOrder < OrderMin)
                            {
                                QtyToOrder = OrderMin;
                            }

                            if ((QtyOnHand + QtyToOrder) > levelMax && levelMax > 0)
                            {
                                QtyToOrder = (levelMax - QtyOnHand);
                            }

                            object qtyPack = DB.ExecuteScalar("SELECT  " + QtyToOrder + " - MOD(" + QtyToOrder + ", " + orderPack + ") + " + orderPack + " FROM DUAL "
                            + " WHERE MOD( " + QtyToOrder + ", " + orderPack + ") <> 0");
                            if (qtyPack != null)
                            {
                                QtyToOrder = Util.GetValueOfDecimal(qtyPack);
                            }
                            qtyPack = (DB.ExecuteScalar("SELECT  " + QtyToOrder + " - MOD(" + QtyToOrder + ", " + orderPack + ") FROM DUAL "
                            + " WHERE MOD( " + QtyToOrder + ", " + orderPack + ") <> 0"));
                            if (qtyPack != null)
                            {
                                QtyToOrder = Util.GetValueOfDecimal(qtyPack);
                            }
                        }
                        else
                        {
                            QtyToOrder = 0;
                        }
                        if (QtyToOrder < 0)
                        {
                            QtyToOrder = 0;
                        }

                        //if (whOrderQty == -999999999)
                        //{
                        //    whOrderQty = 0;
                        //}

                        whOrderQty = whOrderQty + QtyToOrder;
                        // OrderedQty.Add(QtyToOrder);
                    }
                    else
                    {
                        sb.Clear();
                        sb.Append("SELECT COALESCE(SUM(QtyOnHand),0) AS QtyOnHand,COALESCE(SUM(QtyReserved),0) AS QtyReserved,COALESCE(SUM(QtyOrdered),0) AS QtyOrdered FROM M_Storage s, M_Locator l WHERE "
                            + " l.M_Locator_ID=s.M_Locator_ID AND s.M_Product_ID = " + columns[i]);
                        ds = new DataSet();
                        ds = DB.ExecuteDataset(sb.ToString(), null, null);
                        if (ds != null && ds.Tables[0].Rows.Count > 0)
                        {
                            QtyOnHand = Util.GetValueOfDecimal(ds.Tables[0].Rows[0]["QtyOnHand"]);
                            QtyReserved = Util.GetValueOfDecimal(ds.Tables[0].Rows[0]["QtyReserved"]);
                            QtyOrdered = Util.GetValueOfDecimal(ds.Tables[0].Rows[0]["QtyOrdered"]);
                        }

                        if (ReplenishType == 1)
                        {
                            QtyToOrder = levelMin - QtyOnHand + QtyReserved + DTD_SourceReserve - QtyOrdered - DTD_QtyReserved;
                        }
                        else if (ReplenishType == 2)
                        {
                            QtyToOrder = levelMax - QtyOnHand + QtyReserved + DTD_SourceReserve - QtyOrdered - DTD_QtyReserved;
                        }
                        else
                        {
                            QtyToOrder = 0;
                        }
                        if (QtyToOrder < 0)
                        {
                            QtyToOrder = 0;
                        }
                        whOrderQty = whOrderQty + QtyToOrder;

                    }
                }
                if (!found)
                {
                    OrderedQty.Add(-999999999);
                }
                else
                {
                    OrderedQty.Add(whOrderQty);
                }
            }
            return OrderedQty;
        }
        /// <summary>
        /// GetReplenishments Method
        /// </summary>
        /// <param name="Warehouses">Warehouses</param>
        /// <param name="M_Product_ID">M_Product_ID</param>
        /// <param name="ct">ct</param>
        /// <returns>Replenishments</returns>
        public List<RepGet> GetReplenishments(List<int> Warehouses, int M_Product_ID, Ctx ct)
        {
            //DataSet dsRep = new DataSet();
            List<RepGet> repGets = new List<RepGet>();
            StringBuilder sqlRep = new StringBuilder("");
            if (Warehouses != null && Warehouses.Count > 0)
            {
                for (var w = 0; w < Warehouses.Count; w++)
                {
                    sqlRep.Clear();
                    sqlRep.Append("SELECT r.DocumentNo, r.datedoc, rl.Qty, rl.M_Product_ID, rl.DTD001_DeliveredQty, CASE WHEN (rl.Qty - rl.DTD001_DeliveredQty) > 0 THEN (rl.Qty - rl.DTD001_DeliveredQty) ELSE 0 END AS QtyPending "
                    + " FROM m_requisitionline rl INNER JOIN M_Requisition r ON (r.M_Requisition_ID = rl.M_Requisition_ID) INNER JOIN m_product p ON (p.M_Product_ID = rl.M_Product_ID) WHERE rl.M_Product_ID = " + M_Product_ID
                    + " AND r.M_Warehouse_ID = " + Warehouses[w]);
                    DataSet ds = DB.ExecuteDataset(sqlRep.ToString(), null, null);

                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        //string ab = ds.Tables[0].Rows[i]["DocumentNo"].ToString();
                        //dsRep.Tables[0].ImportRow(ds.Tables[0].Rows[i]);
                        RepGet obj = new RepGet();
                        obj.RequisitionNo = Util.GetValueOfInt(ds.Tables[0].Rows[i]["DocumentNo"]);
                        obj.Date = Util.GetValueOfDateTime(ds.Tables[0].Rows[i]["datedoc"]).Value.ToShortDateString();
                        obj.QtyDemanded = Util.GetValueOfInt(ds.Tables[0].Rows[i]["Qty"]);
                        obj.QtyReceived = Util.GetValueOfInt(ds.Tables[0].Rows[i]["DTD001_DeliveredQty"]);
                        obj.QtyPending = Util.GetValueOfInt(ds.Tables[0].Rows[i]["QtyPending"]);
                        obj.M_Product_ID = Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_Product_ID"]);
                        repGets.Add(obj);
                    }
                }
            }
            else
            {
                sqlRep.Clear();
                sqlRep.Append("SELECT M_Warehouse_ID FROM M_Warehouse WHERE AD_Client_ID = " + ct.GetAD_Client_ID() + " AND IsActive = 'Y'");

                DataSet dsWH = DB.ExecuteDataset(sqlRep.ToString(), null, null);
                if (dsWH != null && dsWH.Tables[0].Rows.Count > 0)
                {
                    for (var i = 0; i < dsWH.Tables[0].Rows.Count; i++)
                    {
                        Warehouses.Add(Util.GetValueOfInt(dsWH.Tables[0].Rows[i]["M_Warehouse_ID"]));
                    }

                    for (var w = 0; w < Warehouses.Count; w++)
                    {
                        sqlRep.Clear();
                        sqlRep.Append("SELECT r.DocumentNo, r.datedoc, rl.Qty, rl.M_Product_ID, rl.DTD001_DeliveredQty, CASE WHEN (rl.Qty - rl.DTD001_DeliveredQty) > 0 THEN (rl.Qty - rl.DTD001_DeliveredQty) ELSE 0 END AS QtyPending "
                        + " FROM m_requisitionline rl INNER JOIN M_Requisition r ON (r.M_Requisition_ID = rl.M_Requisition_ID) INNER JOIN m_product p ON (p.M_Product_ID = rl.M_Product_ID) WHERE rl.M_Product_ID = " + M_Product_ID);
                        DataSet ds = DB.ExecuteDataset(sqlRep.ToString(), null, null);
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            //dsRep.Tables[0].ImportRow(ds.Tables[0].Rows[i]);
                            RepGet obj = new RepGet();
                            obj.RequisitionNo = Util.GetValueOfInt(ds.Tables[0].Rows[i]["DocumentNo"]);
                            obj.Date = Util.GetValueOfDateTime(ds.Tables[0].Rows[i]["datedoc"]).Value.ToShortDateString();
                            obj.QtyDemanded = Util.GetValueOfInt(ds.Tables[0].Rows[i]["Qty"]);
                            obj.QtyReceived = Util.GetValueOfInt(ds.Tables[0].Rows[i]["DTD001_DeliveredQty"]);
                            obj.QtyPending = Util.GetValueOfInt(ds.Tables[0].Rows[i]["QtyPending"]);
                            obj.M_Product_ID = Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_Product_ID"]);
                            repGets.Add(obj);
                        }
                    }
                }
            }

            //return dsRep;
            return repGets;
        }
        /// <summary>
        /// GetProductDetails Method
        /// </summary>
        /// <param name="M_Product_ID">M_Product_ID</param>
        /// <param name="ct">ct</param>
        /// <returns>Product Details</returns>
        public DataSet GetProductDetails(int M_Product_ID, Ctx ct)
        {
            string sql = @"SELECT DISTINCT img.ImageURL, p.Name, p.GUARANTEEDAYS,
                p.Weight,
  p.Volume,
  NVL(p.UPC, ' ') as UPC,
  NVL(a.Name,' ') AS ATTRIBUTE,
  u.name AS UOM,
  l.Value as LOCATOR
FROM m_product p
LEFT OUTER JOIN C_UOM u
ON u.C_UOM_ID = p.C_UOM_ID
LEFT OUTER JOIN M_Locator l
ON l.M_Locator_ID = p.M_Locator_ID
LEFT OUTER JOIN M_AttributeSet a
ON a.M_AttributeSet_ID = p.M_AttributeSet_ID
LEFT OUTER JOIN AD_Image img
ON p.AD_Image_ID = img.AD_Image_ID
WHERE M_Product_ID = " + M_Product_ID;
            DataSet dsProd = DB.ExecuteDataset(sql, null, null);
            if (dsProd != null && dsProd.Tables[0] != null)
            {
                foreach (DataColumn column in dsProd.Tables[0].Columns)
                {
                    column.ColumnName = column.ColumnName.ToUpper();
                }
            }
            return dsProd;
        }
        /// <summary>
        /// SaveReplenishment Method
        /// </summary>
        /// <param name="M_Product_ID">M_Product_ID</param>
        /// <param name="M_Warehouse_ID">M_Warehouse_ID</param>
        /// <param name="Type">Type</param>
        /// <param name="Min">Min</param>
        /// <param name="Max">Max</param>
        /// <param name="Qty">Qty</param>
        /// <param name="OrderPack">OrderPack</param>
        /// <param name="SourceWarehoue">SourceWarehoue</param>
        /// <param name="ct">ct</param>
        /// <returns>Replenishment</returns>
        public string SaveReplenishment(int M_Product_ID, int M_Warehouse_ID, string Type, decimal Min, decimal Max, decimal Qty, decimal OrderPack, int SourceWarehoue, Ctx ct)
        {
            string res = "-1";

            MReplenish rep = null;

            string sql = "SELECT * FROM  M_Replenish WHERE M_Warehouse_ID = " + M_Warehouse_ID + " AND M_Product_ID = " + M_Product_ID;
            IDataReader idr = null;
            DataTable dt = null;
            try
            {
                idr = DB.ExecuteReader(sql, null, null);
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    rep = new MReplenish(ct, dr, null);
                }
                if (rep == null)
                {
                    rep = new MReplenish(ct, 0, null);
                    rep.SetM_Product_ID(M_Product_ID);
                    rep.SetM_Warehouse_ID(M_Warehouse_ID);
                }
                rep.SetReplenishType(Type);
                rep.SetDTD001_MinOrderQty(Qty);
                rep.SetDTD001_OrderPackQty(OrderPack);
                rep.SetLevel_Max(Max);
                rep.SetLevel_Min(Min);
                if (SourceWarehoue != 0)
                {
                    rep.SetM_WarehouseSource_ID(SourceWarehoue);
                }
                if (rep.Save())
                {

                }

            }
            catch (Exception ex)
            {
                if (idr != null)
                {
                    idr.Close();
                    idr = null;
                }
            }

            return res;
        }
        /// <summary>
        /// saveInventoryCount Method
        /// </summary>
        /// <param name="columnName">columnName</param>
        /// <param name="ctx">ctx</param>
        /// <returns>Inventory Count</returns>
        public string saveInventoryCount(string columnName, Ctx ctx)
        {
            string error = "";
            MVAICNTInventoryCount cnt = new MVAICNTInventoryCount(ctx, 0, null);
            cnt.SetVAICNT_TransactionType("OT");
            cnt.SetVAICNT_ScanName(columnName);
            if (!cnt.Save())
            {
                //ValueNamePair pp = VLogger.RetrieveError();
                //error = pp.ToString();
            }
            else
            {
                error = cnt.Get_ID().ToString();
            }

            return error;
        }
        /// <summary>
        /// saveInventory Method
        /// </summary>
        /// <param name="count_id">count_id</param>
        /// <param name="columnName">columnName</param>
        /// <param name="ctx">ctx</param>
        /// <returns>Inventory</returns>
        public string saveInventory(int count_id, List<PriceInfo> columnName, Ctx ctx)
        {
            string error = "";
            string qry = "";
            int lineID = 0;
            string upc = "";
            MProduct pro = null;
            for (int i = 0; i < columnName.Count; i++)
            {
                if (String.IsNullOrEmpty(columnName[i].UPC))
                {
                    upc = " ";
                }
                else
                {
                    upc = Util.GetValueOfString(columnName[i].UPC);
                }
                qry = "SELECT VAICNT_InventoryCountLine_ID FROM VAICNT_InventoryCountLine WHERE M_Product_ID = " + columnName[i].product_ID + " AND VAICNT_InventoryCount_ID=" + count_id +
                    " AND NVL(C_UOM_ID,0) = " + columnName[i].C_Uom_ID + " AND NVL(M_AttributeSetInstance_ID,0) = " + columnName[i].attribute_ID + " AND nvl(UPC,' ') ='" + upc + "'";
                lineID = Util.GetValueOfInt(DB.ExecuteScalar(qry, null, null));
                MVAICNTInventoryCountLine iline = new MVAICNTInventoryCountLine(ctx, lineID, null);
                pro = new MProduct(ctx, columnName[i].product_ID, null);
                if (lineID > 0)
                {
                    iline.SetC_UOM_ID(columnName[i].C_Uom_ID);
                    iline.SetM_AttributeSetInstance_ID(columnName[i].attribute_ID);
                    iline.SetUPC(columnName[i].UPC);
                    iline.SetVAICNT_Quantity(iline.GetVAICNT_Quantity() + columnName[i].Qty);
                }
                else
                {
                    iline.SetVAICNT_InventoryCount_ID(count_id);
                    iline.SetM_Product_ID(columnName[i].product_ID);
                    iline.SetC_UOM_ID(columnName[i].C_Uom_ID);
                    iline.SetM_AttributeSetInstance_ID(columnName[i].attribute_ID);
                    iline.SetUPC(columnName[i].UPC);
                    iline.SetVAICNT_Quantity(columnName[i].Qty);
                }
                if (!iline.Save())
                {
                    ValueNamePair pp = VLogger.RetrieveError();
                    error += pro.GetName() + " - " + pp.ToString() + "\n";
                }
            }
            return error;
        }
        /// <summary>
        /// updateInventory Method
        /// </summary>
        /// <param name="columnName">columnName</param>
        /// <param name="ctx">ctx</param>
        /// <returns>Inventory</returns>
        public string updateInventory(List<PriceInfo> columnName, Ctx ctx)
        {
            string error = "";
            int lineID = 0;
            MProduct pro = null;
            for (int i = 0; i < columnName.Count; i++)
            {
                if (columnName[i].Qty <= 0)
                {
                    continue;
                }
                lineID = columnName[i].LineID;
                pro = new MProduct(ctx, columnName[i].product_ID, null);
                MVAICNTInventoryCountLine iline = new MVAICNTInventoryCountLine(ctx, lineID, null);
                iline.SetC_UOM_ID(columnName[i].C_Uom_ID);
                iline.SetM_AttributeSetInstance_ID(columnName[i].attribute_ID);
                iline.SetUPC(columnName[i].UPC);
                iline.SetVAICNT_Quantity(columnName[i].Qty);
                if (!iline.Save())
                {
                    ValueNamePair pp = VLogger.RetrieveError();
                    error += pro.GetName() + " - " + pp.ToString() + "\n";
                }
            }
            return error;
        }
        /// <summary>
        /// deleteInventory Method
        /// </summary>
        /// <param name="columnName">columnName</param>
        /// <param name="ctx">ctx</param>
        /// <returns></returns>
        public string deleteInventory(List<PriceInfo> columnName, Ctx ctx)
        {
            string error = "";
            string qry = "";
            int lineID = 0;
            int no = 0;
            MProduct pro = null;
            for (int i = 0; i < columnName.Count; i++)
            {
                pro = new MProduct(ctx, columnName[i].product_ID, null);
                lineID = columnName[i].LineID;
                qry = "DELETE FROM VAICNT_InventoryCountLine WHERE VAICNT_InventoryCountLine_ID = " + lineID;
                no = DB.ExecuteQuery(qry, null, null);
                if (no <= 0)
                {
                    error += Msg.GetMsg(ctx, "DeleteError") + pro.GetName() + "\n";
                }
            }
            return error;
        }
        /// <summary>
        /// GenerateReplenishmentReport Method
        /// </summary>
        /// <param name="M_Warehouse_ID">M_Warehouse_ID</param>
        /// <param name="C_BPartner_ID">C_BPartner_ID</param>
        /// <param name="C_DocType_ID">C_DocType_ID</param>
        /// <param name="DocStatus">DocStatus</param>
        /// <param name="Create">Create</param>
        /// <param name="OrderPack">OrderPack</param>
        /// <param name="ct">ct</param>
        /// <returns>Replenishment Report</returns>
        public DataSet GenerateReplenishmentReport(int M_Warehouse_ID, int C_BPartner_ID, int C_DocType_ID, string DocStatus, string Create, string OrderPack, Ctx ct)
        {
            ReplenishmentReport repR = new ReplenishmentReport();
            DataSet dsRep = repR.GenerateReplenishmentReport(M_Warehouse_ID, C_BPartner_ID, C_DocType_ID, DocStatus, Create, OrderPack, ct);
            return dsRep;
        }

        VAdvantage.Model.MOrder orderReps = null;
        VAdvantage.Model.MMovement moveReps = null;
        VAdvantage.Model.MRequisition requisitionReps = null;
        string docStatusReps = "DR";
        bool gotDocStatus = false;
        List<int> createdRecordReps = new List<int>();
        string RepCreate = "";
        string _DocNo = "";
        StringBuilder sbRetMsg = new StringBuilder("");
        static VLogger log = VLogger.GetVLogger("StockManagement");
        int M_Warehouse_ID = 0;
        int M_WarehouseSource_ID = 0;

        /// <summary>
        /// GenerateReps Method
        /// </summary>
        /// <param name="Reps">Reps</param>
        /// <param name="ct">ct</param>
        /// <returns>Reps</returns>
        public string GenerateReps(List<RepCreateData> Reps, Ctx ct)
        {
            string storedPath = Path.Combine(HostingEnvironment.ApplicationPhysicalPath, "");

            VLogMgt.Initialize(true, storedPath);

            MWarehouse wh = null;
            VAdvantage.Model.MBPartner bp = null; //v

            List<String> docCreated = new List<string>();

            Trx trx = Trx.GetTrx("VA011_trxRepGen" + System.DateTime.Now.Ticks);
            bool allOK = true;

            for (int i = 0; i < Reps.Count; i++)
            {
                if (Reps[i].RepCreate == "POO")
                {
                    RepCreate = Reps[i].RepCreate;
                    if (i == 0)
                    {
                        sbRetMsg.Append(Msg.GetMsg(ct, "VA011_PurcahseOrderNo"));
                    }

                    log.SaveInfo("StockMgmt PO 1", "StockMgmt PO 1");
                    string docNo = CreatePO(Reps[i], wh, bp, ct, trx);
                    log.SaveInfo("StockMgmt PO 2", "StockMgmt PO 2");
                    if (docNo == "-1")
                    {
                        allOK = false;
                        sbRetMsg.Clear().Append(Msg.GetMsg(ct, "VA011_BusinessPartnerNotFound"));
                        trx.Rollback();
                        trx.Close();
                        break;
                    }
                    else if (docNo == "-99")
                    {
                        allOK = false;
                        sbRetMsg.Clear().Append(Msg.GetMsg(ct, "VA011_PurchaseOrderNotCreated"));
                        trx.Rollback();
                        trx.Close();
                        break;
                    }
                    else
                    {
                        if (!docCreated.Contains(docNo))
                        {
                            docCreated.Add(docNo);
                        }
                    }
                }
                else if (Reps[i].RepCreate == "POR")
                {
                    RepCreate = Reps[i].RepCreate;
                    if (i == 0)
                    {
                        sbRetMsg.Append(Msg.GetMsg(ct, "VA011_RequisitionNo"));
                    }

                    string docNo = CreateRequisition(Reps[i], wh, bp, ct, trx);
                    if (docNo == "-99")
                    {
                        allOK = false;
                        //sbRetMsg.Clear().Append(Msg.GetMsg(ct, "VA011_RequisitionNotCreated"));
                        trx.Rollback();
                        trx.Close();
                        break;
                    }
                    else
                    {
                        if (!docCreated.Contains(docNo))
                        {
                            docCreated.Add(docNo);
                        }
                    }
                }
                else if (Reps[i].RepCreate == "MMM")
                {
                    RepCreate = Reps[i].RepCreate;
                    if (i == 0)
                    {
                        sbRetMsg.Append(Msg.GetMsg(ct, "VA011_InventoryMoveNo"));
                    }
                    string docNo = CreateMovements(Reps[i], wh, bp, ct, trx);
                    if (docNo == "-1")
                    {
                        allOK = false;
                        sbRetMsg.Clear().Append(Msg.GetMsg(ct, "VA011_StorageNotFound"));
                        trx.Rollback();
                        trx.Close();
                        break;
                    }
                    else if (docNo == "-99")
                    {
                        allOK = false;
                        sbRetMsg.Clear().Append(Msg.GetMsg(ct, "VA011_MoveNotCreated"));
                        trx.Rollback();
                        trx.Close();
                        break;
                    }
                    else
                    {
                        if (!docCreated.Contains(docNo))
                        {
                            docCreated.Add(docNo);
                        }
                    }
                }
            }
            gotDocStatus = false;

            if (allOK)
            {
                if (docStatusReps == "IP" || docStatusReps == "CO")
                {
                    if (createdRecordReps.Count > 0)
                    {
                        if (RepCreate == "POO")
                        {
                            int[] array = createdRecordReps.ToArray();
                            for (int k = 0; k < array.Length; k++)
                            {
                                orderReps = new VAdvantage.Model.MOrder(ct, Util.GetValueOfInt(array[k]), trx);
                                if (docStatusReps == "IP")
                                {
                                    orderReps.SetDocStatus("IP");
                                    orderReps.Save();
                                    orderReps.PrepareIt();
                                    if (!orderReps.Save())
                                    {
                                        allOK = false;
                                        sbRetMsg.Clear().Append(Msg.GetMsg(ct, "VA011_OrderNotSaved"));
                                        trx.Rollback();
                                        trx.Close();
                                        break;
                                    }
                                }
                                else if (docStatusReps == "CO")
                                {
                                    orderReps.SetDocStatus("CO");
                                    orderReps.SetDocAction("CL");
                                    orderReps.Save();
                                    orderReps.CompleteIt();
                                    if (!orderReps.Save())
                                    {
                                        sbRetMsg.Clear().Append(Msg.GetMsg(ct, "VA011_OrderNotSaved"));
                                        allOK = false;
                                        trx.Rollback();
                                        trx.Close();
                                        break;
                                    }
                                }
                            }
                        }
                        else if (RepCreate == "POR")
                        {
                            int[] array = createdRecordReps.ToArray();
                            for (int k = 0; k < array.Length; k++)
                            {
                                requisitionReps = new VAdvantage.Model.MRequisition(ct, Util.GetValueOfInt(array[k]), trx);
                                if (docStatusReps == "IP")
                                {
                                    requisitionReps.PrepareIt();
                                    requisitionReps.SetDocStatus("IP");
                                }
                                if (docStatusReps == "CO")
                                {
                                    string chk = requisitionReps.CompleteIt();
                                    if (chk == "CO")
                                    {
                                        requisitionReps.SetDocStatus("CO");
                                    }
                                }
                                if (!requisitionReps.Save())
                                {
                                    allOK = false;
                                    sbRetMsg.Clear().Append(Msg.GetMsg(ct, "VA011_RequisitionNotSaved"));
                                    trx.Rollback();
                                    trx.Close();
                                }
                            }
                        }
                        else if (RepCreate == "MMM")
                        {
                            int[] array = createdRecordReps.ToArray();
                            for (int k = 0; k < array.Length; k++)
                            {
                                moveReps = new VAdvantage.Model.MMovement(ct, Util.GetValueOfInt(array[k]), trx);
                                if (docStatusReps == "IP")
                                {
                                    moveReps.SetDocStatus("IP");
                                    moveReps.Save();
                                    moveReps.PrepareIt();
                                    if (!moveReps.Save())
                                    {
                                        sbRetMsg.Clear().Append(Msg.GetMsg(ct, "VA011_MoveNotSaved"));
                                        allOK = false;
                                        trx.Rollback();
                                        trx.Close();
                                        break;
                                    }
                                }
                                else if (docStatusReps == "CO")
                                {
                                    moveReps.SetDocStatus("CO");
                                    moveReps.SetDocAction("CL");
                                    moveReps.Save();
                                    moveReps.CompleteIt();
                                    if (!moveReps.Save())
                                    {
                                        sbRetMsg.Clear().Append(Msg.GetMsg(ct, "VA011_MoveNotSaved"));
                                        allOK = false;
                                        trx.Rollback();
                                        trx.Close();
                                        break;
                                    }
                                }
                            }

                            string query = @"DELETE FROM m_movement WHERE m_movement_id IN   (SELECT m_movement_id   FROM M_movement   WHERE m_movement_id NOT IN
                                                       (SELECT DISTINCT m_movement_id FROM m_movementline     )   )";
                            int j = DB.ExecuteQuery(query.ToString(), null, trx);
                            if (j < 0)
                            {
                                //log.Info("Inventory Move not deleted where movementline not created ");
                            }
                        }
                    }
                }
            }
            //orderReps = null;

            if (allOK)
            {
                trx.Commit();
                trx.Close();
                if (docCreated.Count > 0)
                {
                    for (int i = 0; i < docCreated.Count; i++)
                    {
                        if (i == 0)
                        {
                            sbRetMsg.Append(" : ");

                        }
                        sbRetMsg.Append(docCreated[i] + " ,");
                        if (i == docCreated.Count - 1)
                        {
                            string msg = sbRetMsg.ToString();
                            msg = msg.Substring(0, msg.Length - 1);
                            sbRetMsg.Clear().Append(msg);

                            sbRetMsg.Append(" " + Msg.GetMsg(ct, "VA011_CreatedSuccessfully"));
                        }
                    }

                }
                else
                {
                    sbRetMsg.Clear().Append(Msg.GetMsg(ct, "VA011_NoDocumentCreated"));
                }
            }

            return sbRetMsg.ToString();
        }

        /// <summary>
        /// Create PO's
        /// </summary>
        private string CreatePO(RepCreateData Rep, MWarehouse wh, MBPartner bp, Ctx ct, Trx _trx)
        {
            int noOrders = 0;
            String info = "";
            // int _CountED008 = 0;
            int _CountTaxType = 0;
            int _VLTaxType_ID = 0;
            int _VTaxType_ID = 0;
            int _TaxCtgry_ID = 0;
            int _TaxID = 0;
            String _qry = null;

            if (!gotDocStatus)
            {
                docStatusReps = Rep.DocStatus;
                gotDocStatus = true;
            }
            log.SaveInfo("StockMgmt PO 3", "StockMgmt PO 3");
            if (wh == null || wh.GetM_Warehouse_ID() != Rep.M_Warehouse_ID)
            {
                wh = MWarehouse.Get(ct, Rep.M_Warehouse_ID);
            }
            log.SaveInfo("StockMgmt PO 4", "StockMgmt PO 4");
            if (Rep.C_BPartner_ID != 0)
            {
                if (orderReps == null || orderReps.GetC_BPartner_ID() != Rep.C_BPartner_ID || orderReps.GetM_Warehouse_ID() != wh.GetM_Warehouse_ID())
                {
                    log.SaveInfo("StockMgmt PO 5", "StockMgmt PO 5");
                    // order = new MOrder(GetCtx(), 0, Get_Trx());
                    orderReps = new VAdvantage.Model.MOrder(ct, 0, _trx);
                    orderReps.SetIsSOTrx(false);
                    orderReps.SetC_DocTypeTarget_ID(Rep.C_DocType_ID);
                    orderReps.SetC_DocType_ID(Rep.C_DocType_ID);
                    // MBPartner bp = new MBPartner(GetCtx(), replenish.GetC_BPartner_ID(), Get_Trx());
                    bp = new VAdvantage.Model.MBPartner(ct, Rep.C_BPartner_ID, _trx);
                    orderReps.SetBPartner(bp);
                    orderReps.SetVA009_PaymentMethod_ID(bp.GetVA009_PO_PaymentMethod_ID());
                    orderReps.SetSalesRep_ID(ct.GetAD_User_ID());
                    orderReps.SetDescription(Msg.GetMsg(ct, "Replenishment"));
                    //	Set Org/WH
                    orderReps.SetAD_Org_ID(wh.GetAD_Org_ID());
                    orderReps.SetM_Warehouse_ID(wh.GetM_Warehouse_ID());
                    log.SaveInfo("StockMgmt PO 6", "StockMgmt PO 6");
                    if (!orderReps.Save())
                    {
                        log.SaveInfo("StockMgmt PO 7", "StockMgmt PO 7");
                        return "-99";
                    }
                    else
                    {
                        log.SaveInfo("StockMgmt PO 8", "StockMgmt PO 8");
                        if (!createdRecordReps.Contains(orderReps.GetC_Order_ID()))
                        {
                            createdRecordReps.Add(orderReps.GetC_Order_ID());
                        }
                    }
                    noOrders++;
                    info += " - " + orderReps.GetDocumentNo();
                }

                log.SaveInfo("StockMgmt PO 9", "StockMgmt PO 9");

                // MOrderLine line = new MOrderLine(order);
                VAdvantage.Model.MOrderLine line = null;
                line = new VAdvantage.Model.MOrderLine(orderReps);
                line.SetM_Product_ID(Rep.M_Product_ID);
                line.SetM_AttributeSetInstance_ID(Rep.M_AttributeSetInstance_ID);
                //line.SetQty(replenish.GetQtyToOrder());
                int UOM = 0, prdUOM = 0;
                decimal? OrdQty = 0, OrignlQty = 0;
                double Discount = 0;
                DataSet prodDs = DB.ExecuteDataset("SELECT C_UOM_ID, DocumentNote FROM  M_Product prdct WHERE prdct.isactive='Y' AND prdct.M_Product_ID=" + Rep.M_Product_ID);
                if (prodDs != null && prodDs.Tables.Count > 0 && prodDs.Tables[0].Rows.Count > 0)
                {
                    prdUOM = Util.GetValueOfInt(prodDs.Tables[0].Rows[0]["C_UOM_ID"]);
                    //190- Set the print desc.
                    if (line.Get_ColumnIndex("PrintDescription") >= 0)
                        line.Set_Value("PrintDescription", Util.GetValueOfString(prodDs.Tables[0].Rows[0]["DocumentNote"]));
                }
                UOM = Util.GetValueOfInt(DB.ExecuteScalar("SELECT C_UOM_ID  FROM  M_Product_PO prdct WHERE prdct.isactive='Y' AND prdct.M_Product_ID=" + Rep.M_Product_ID));

                if (UOM == 0)
                {
                    UOM = prdUOM;
                }

                #region Calculate Quantity
                OrdQty = Rep.QtyToOrder;
                if (prdUOM != UOM)
                {
                    decimal? Res = Util.GetValueOfDecimal(DB.ExecuteScalar("SELECT trunc(multiplyrate,4) FROM C_UOM_Conversion WHERE C_UOM_ID = " + prdUOM + " AND C_UOM_To_ID = " + UOM + " AND M_Product_ID= " + Rep.M_Product_ID, null, null));
                    if (Res > 0)
                    {
                        OrignlQty = OrdQty;
                        OrdQty = OrdQty * Res;
                        //OrdQty = MUOMConversion.ConvertProductTo(GetCtx(), _M_Product_ID, UOM, OrdQty);
                    }
                    else
                    {
                        decimal? res = Util.GetValueOfDecimal(DB.ExecuteScalar("SELECT trunc(multiplyrate,4) FROM C_UOM_Conversion WHERE C_UOM_ID = " + prdUOM + " AND C_UOM_To_ID = " + UOM, null, null));
                        if (res > 0)
                        {
                            OrignlQty = OrdQty;
                            OrdQty = OrdQty * res;
                            // OrdQty = MUOMConversion.Convert(GetCtx(), prdUOM, UOM, OrdQty);
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

                log.SaveInfo("StockMgmt PO 10", "StockMgmt PO 10");
                line.SetPrice();
                #region Set TaxID  on PoLine if TaxType Module Downloaded 1/Dec/2014 (vikas)
                _CountTaxType = Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(AD_MODULEINFO_ID) FROM AD_MODULEINFO WHERE PREFIX='VATAX_'"));
                if (_CountTaxType > 0)
                {
                    //Get TaxCategory from Product  & TaxType from VendorLocation
                    _TaxCtgry_ID = Util.GetValueOfInt(DB.ExecuteScalar("SELECT C_TaxCategory_ID  FROM  M_Product prdct WHERE prdct.isactive='Y' AND prdct.M_Product_ID=" + Rep.M_Product_ID));
                    _VLTaxType_ID = Util.GetValueOfInt(DB.ExecuteScalar("SELECT VATAX_TaxType_ID  FROM  C_BPartner_Location bpl WHERE bpl.isactive='Y' AND  bpl.C_BPartner_ID=" + orderReps.GetC_BPartner_ID() + " and bpl.C_BPartner_Location_ID=" + orderReps.GetC_BPartner_Location_ID()));
                    if (_VLTaxType_ID > 0)
                    {
                        _TaxID = Util.GetValueOfInt(DB.ExecuteScalar("SELECT TaxRate.c_tax_id FROM C_TaxCategory Taxctgry INNER JOIN VATAX_TaxCatRate TaxRate ON(Taxctgry.C_TaxCategory_ID=TaxRate.C_TaxCategory_ID)  WHERE Taxctgry.isactive='Y'  AND Taxctgry.C_TaxCategory_ID=" + _TaxCtgry_ID + " AND TaxRate.VATAX_TaxType_ID=" + _VLTaxType_ID));
                        if (_TaxID != 0)
                        {
                            line.SetC_Tax_ID(_TaxID);
                        }
                    }
                    else
                    {
                        // Get TaxType From VendorHeader
                        _VTaxType_ID = Util.GetValueOfInt(DB.ExecuteScalar("SELECT VATAX_TaxType_ID  FROM  C_BPartner bp WHERE bp.isactive='Y' AND  bp.C_BPartner_ID=" + orderReps.GetC_BPartner_ID()));
                        if (_VTaxType_ID > 0)
                        {
                            _TaxID = Util.GetValueOfInt(DB.ExecuteScalar("SELECT TaxRate.c_tax_id FROM C_TaxCategory Taxctgry INNER JOIN VATAX_TaxCatRate TaxRate ON(Taxctgry.C_TaxCategory_ID=TaxRate.C_TaxCategory_ID)  WHERE Taxctgry.isactive='Y'  AND Taxctgry.C_TaxCategory_ID=" + _TaxCtgry_ID + " AND TaxRate.VATAX_TaxType_ID=" + _VTaxType_ID));
                            if (_TaxID != 0)
                            {
                                line.SetC_Tax_ID(_TaxID);
                            }
                        }
                    }
                    orderReps.GetC_BPartner_Location_ID();

                }
                #endregion
                log.SaveInfo("StockMgmt PO 11", "StockMgmt PO 11");
                #region  Set Price  1/Dec/2014
                decimal? ListPrice = 0, Price = 0, UnitPrice = 0;
                int PriceListVersion = Util.GetValueOfInt(DB.ExecuteScalar("select max(m_pricelist_version_id) from m_pricelist_version where validfrom<=sysdate  and isactive='Y' and m_pricelist_id=" + orderReps.GetM_PriceList_ID()));
                StringBuilder SQL = new StringBuilder();
                StringBuilder SqlUom = new StringBuilder();
                DataSet DsPrice = new DataSet();
                SQL.Append(@"SELECT PriceList , PriceStd , PriceLimit FROM M_ProductPrice WHERE Isactive='Y' AND M_Product_ID = " + Rep.M_Product_ID
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
                    SQL.Append(" AND M_AttributeSetInstance_ID = " + Rep.M_AttributeSetInstance_ID);
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
                            SqlUom.Append("AND C_UOM_ID=" + prdUOM);
                            DsPrice = DB.ExecuteDataset(SqlUom.ToString());
                            if (DsPrice.Tables[0].Rows.Count > 0)
                            {
                                ListPrice = Util.GetValueOfDecimal(DsPrice.Tables[0].Rows[0]["PriceList"]);
                                UnitPrice = Util.GetValueOfDecimal(DsPrice.Tables[0].Rows[0]["PriceStd"]);
                                Price = UnitPrice;
                                if (prdUOM != UOM)
                                {
                                    //decimal? Res = Util.GetValueOfDecimal(DB.ExecuteScalar("SELECT trunc(multiplyrate,4) FROM C_UOM_Conversion WHERE C_UOM_ID = " + prdUOM + " AND C_UOM_To_ID = " + UOM + " AND M_Product_ID= " + replenish.GetM_Product_ID(), null, Get_Trx()));
                                    decimal? Res = Util.GetValueOfDecimal(DB.ExecuteScalar("SELECT trunc(DivideRate,4) FROM C_UOM_Conversion WHERE C_UOM_ID = " + prdUOM + " AND C_UOM_To_ID = " + UOM + " AND M_Product_ID= " + Rep.M_Product_ID, null, null));
                                    if (Res > 0)
                                    {
                                        Price = UnitPrice * Res;
                                        UnitPrice = UnitPrice * Res;
                                        ListPrice = ListPrice * Res;

                                        //OrdQty = MUOMConversion.ConvertProductTo(GetCtx(), _M_Product_ID, UOM, OrdQty);
                                    }
                                    else
                                    {
                                        decimal? res = Util.GetValueOfDecimal(DB.ExecuteScalar("SELECT trunc(DivideRate,4) FROM C_UOM_Conversion WHERE C_UOM_ID = " + prdUOM + " AND C_UOM_To_ID = " + UOM, null, null));
                                        if (res > 0)
                                        {

                                            Price = UnitPrice * res;
                                            UnitPrice = UnitPrice * res;
                                            ListPrice = ListPrice * res;
                                            // OrdQty = MUOMConversion.Convert(GetCtx(), prdUOM, UOM, OrdQty);
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
                log.SaveInfo("StockMgmt PO 12", "StockMgmt PO 12");
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
                        SqlUom.Append("AND C_UOM_ID=" + prdUOM);
                        DsPrice = DB.ExecuteDataset(SqlUom.ToString());
                        if (DsPrice.Tables[0].Rows.Count > 0)
                        {
                            ListPrice = Util.GetValueOfDecimal(DsPrice.Tables[0].Rows[0]["PriceList"]);
                            UnitPrice = Util.GetValueOfDecimal(DsPrice.Tables[0].Rows[0]["PriceStd"]);
                            Price = UnitPrice;
                            if (prdUOM != UOM)
                            {
                                decimal? Res = Util.GetValueOfDecimal(DB.ExecuteScalar("SELECT trunc(DivideRate,4) FROM C_UOM_Conversion WHERE C_UOM_ID = " + prdUOM + " AND C_UOM_To_ID = " + UOM + " AND M_Product_ID= " + Rep.M_Product_ID, null, null));
                                if (Res > 0)
                                {
                                    Price = UnitPrice * Res;
                                    UnitPrice = UnitPrice * Res;
                                    ListPrice = ListPrice * Res;
                                    //OrdQty = MUOMConversion.ConvertProductTo(GetCtx(), _M_Product_ID, UOM, OrdQty);
                                }
                                else
                                {
                                    decimal? res = Util.GetValueOfDecimal(DB.ExecuteScalar("SELECT trunc(DivideRate,4) FROM C_UOM_Conversion WHERE C_UOM_ID = " + prdUOM + " AND C_UOM_To_ID = " + UOM, null, null));
                                    if (res > 0)
                                    {
                                        Price = UnitPrice * res;
                                        UnitPrice = UnitPrice * res;
                                        ListPrice = ListPrice * res;
                                        // OrdQty = MUOMConversion.Convert(GetCtx(), prdUOM, UOM, OrdQty);
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
                    decimal? DiscPercent = Util.GetValueOfDecimal(DB.ExecuteScalar("select discount from c_paymentterm where c_paymentterm_id=" + orderReps.GetC_PaymentTerm_ID()));
                    line.SetED007_DiscountPercent(DiscPercent);
                }
                log.SaveInfo("StockMgmt PO 13", "StockMgmt PO 13");
                int precision = MCurrency.GetStdPrecision(ct, orderReps.GetC_Currency_ID());
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
                if (ListPrice != 0)
                {
                    Discount = Util.GetValueOfDouble((Num / ListPrice) * 100);
                }
                Discount = Math.Round(Discount, 2);
                line.SetDiscount(Util.GetValueOfDecimal(Discount));
                log.SaveInfo("StockMgmt PO 14", "StockMgmt PO 14");
                if (!line.Save())
                {
                    //
                }
                return orderReps.GetDocumentNo();
            }
            else
            {
                return "-1";
            }
        }   //	createPO

        /// <summary>
        /// Create Requisition
        /// </summary>
        private string CreateRequisition(RepCreateData Rep, MWarehouse wh, MBPartner bp, Ctx ct, Trx _trx)
        {
            int noReqs = 0;
            String info = "";
            //VAdvantage.Model.MRequisition requisition = null;
            if (wh == null || wh.GetM_Warehouse_ID() != Rep.M_Warehouse_ID)
            {
                wh = MWarehouse.Get(ct, Rep.M_Warehouse_ID);
            }

            if (!gotDocStatus)
            {
                docStatusReps = Rep.DocStatus;
                gotDocStatus = true;
            }

            if (requisitionReps == null   //newchange
               || requisitionReps.GetDTD001_MWarehouseSource_ID() != Rep.M_WarehouseSource_ID)
            {
                requisitionReps = new VAdvantage.Model.MRequisition(ct, 0, _trx);
                requisitionReps.SetAD_User_ID(ct.GetAD_User_ID());
                requisitionReps.SetC_DocType_ID(Rep.C_DocType_ID);
                requisitionReps.SetDescription(Msg.GetMsg(ct, "Replenishment"));
                //	Set Org/WH
                if (Env.IsModuleInstalled("DTD001_"))
                {
                    requisitionReps.SetDTD001_MWarehouseSource_ID(Rep.M_WarehouseSource_ID);
                }
                requisitionReps.SetAD_Org_ID(wh.GetAD_Org_ID());
                requisitionReps.SetM_Warehouse_ID(wh.GetM_Warehouse_ID());

                if (!requisitionReps.Save())
                {
                    sbRetMsg.Clear();
                    ValueNamePair pp = VLogger.RetrieveError();
                    if (pp != null && !String.IsNullOrEmpty(pp.GetName()))
                        sbRetMsg.Append(pp.GetName());
                    else
                        sbRetMsg.Append(Msg.GetMsg(ct, "VA011_RequisitionNotCreated"));
                    return "-99";
                }
                else
                {
                    if (!createdRecordReps.Contains(requisitionReps.GetM_Requisition_ID()))
                    {
                        createdRecordReps.Add(requisitionReps.GetM_Requisition_ID());
                    }
                }
                _DocNo = requisitionReps.GetDocumentNo(); //dtd
                                                          //log.Fine(requisition.ToString());
                noReqs++;
                info += " - " + requisitionReps.GetDocumentNo();
            }

            MProduct product = MProduct.Get(ct, Rep.M_Product_ID);
            VAdvantage.Model.MRequisitionLine line = new VAdvantage.Model.MRequisitionLine(requisitionReps);
            //ViennaAdvantage.Model.MRequisitionLine line = new ViennaAdvantage.Model.MRequisitionLine(GetCtx() , 0 , Get_Trx());
            line.SetM_Requisition_ID(requisitionReps.GetM_Requisition_ID());
            line.SetM_Product_ID(Rep.M_Product_ID);
            line.SetM_AttributeSetInstance_ID(Rep.M_AttributeSetInstance_ID);
            line.SetC_BPartner_ID(Rep.C_BPartner_ID);
            line.SetQty(Rep.QtyToOrder);

            // Set Value in Qty Entered field on Requisition Line
            if (line.Get_ColumnIndex("QtyEntered") > 0)
            {
                line.Set_Value("QtyEntered", Rep.QtyToOrder);
            }

            // Set Value of Product UOM on Requisition Line.
            if (line.Get_ColumnIndex("C_UOM_ID") > 0)
            {
                line.Set_ValueNoCheck("C_UOM_ID", product.GetC_UOM_ID());
            }
            //190- Set the print desc.
            if (line.Get_ColumnIndex("PrintDescription") >= 0)
                line.Set_Value("PrintDescription", product.GetDocumentNote());
            if (!line.Save())
            {
                sbRetMsg.Clear();
                ValueNamePair pp = VLogger.RetrieveError();
                if (pp != null && !String.IsNullOrEmpty(pp.GetName()))
                    sbRetMsg.Append(pp.GetName());
                else
                    sbRetMsg.Append(Msg.GetMsg(ct, "VA011_RequisitionNotCreated"));
                return "-99";
            }

            return _DocNo;
            // }


            // _info = "#" + noReqs + info;
            //  log.Info(_info);
        }   //	createRequisition

        /// <summary>
        /// Create Inventory Movements
        /// </summary>
        private string CreateMovements(RepCreateData Rep, MWarehouse wh, MBPartner bp, Ctx ct, Trx _trx)
        {
            int noMoves = 0;
            String info = "";
            //
            MClient client = null;
            MWarehouse whSource = null;
            MWarehouse whTarget = null;

            if (!gotDocStatus)
            {
                docStatusReps = Rep.DocStatus;
                gotDocStatus = true;
            }

            if (whSource == null || whSource.GetM_WarehouseSource_ID() != Rep.M_WarehouseSource_ID)
            {
                whSource = MWarehouse.Get(ct, Rep.M_WarehouseSource_ID);
            }
            if (whTarget == null || whTarget.GetM_Warehouse_ID() != Rep.M_Warehouse_ID)
            {
                whTarget = MWarehouse.Get(ct, Rep.M_Warehouse_ID);
            }
            if (client == null || client.GetAD_Client_ID() != whSource.GetAD_Client_ID())
            {
                client = MClient.Get(ct, whSource.GetAD_Client_ID());
            }
            //
            if (moveReps == null
                || M_WarehouseSource_ID != Rep.M_WarehouseSource_ID
                || M_Warehouse_ID != Rep.M_Warehouse_ID)
            {
                M_WarehouseSource_ID = Rep.M_WarehouseSource_ID;
                M_Warehouse_ID = Rep.M_Warehouse_ID;

                //if (M_WarehouseSource_ID == 0)
                //{
                //    M_WarehouseSource_ID = whTarget.GetM_WarehouseSource_ID();
                //}

                moveReps = new VAdvantage.Model.MMovement(ct, 0, _trx);
                moveReps.SetC_DocType_ID(Rep.C_DocType_ID);
                moveReps.SetDescription(Msg.GetMsg(ct, "Replenishment")
                    + ": " + whSource.GetName() + "->" + whTarget.GetName());
                //	Set Org            
                moveReps.SetAD_Org_ID(whSource.GetAD_Org_ID());
                moveReps.SetDTD001_MWarehouseSource_ID(M_WarehouseSource_ID);
                moveReps.SetMovementDate(DateTime.Now);
                moveReps.SetM_Warehouse_ID(M_Warehouse_ID);
                if (!moveReps.Save())
                {
                    return "-99";
                }
                else
                {
                    if (!createdRecordReps.Contains(moveReps.GetM_Movement_ID()))
                    {
                        createdRecordReps.Add(moveReps.GetM_Movement_ID());
                    }
                }
                //log.Fine(move.ToString());
                noMoves++;
                info += " - " + moveReps.GetDocumentNo();
            }
            MProduct product = MProduct.Get(ct, Rep.M_Product_ID);
            //	To
            int M_LocatorTo_ID = GetLocator_ID(product, whTarget);

            //	From: Look-up Storage
            MProductCategory pc = MProductCategory.Get(ct, product.GetM_Product_Category_ID());
            String MMPolicy = pc.GetMMPolicy();
            if (MMPolicy == null || MMPolicy.Length == 0)
            {
                MMPolicy = client.GetMMPolicy();
            }
            //
            MStorage[] storages = MStorage.GetWarehouse(ct,
                whSource.GetM_Warehouse_ID(), Rep.M_Product_ID, 0, 0,
                true, null,
                MClient.MMPOLICY_FiFo.Equals(MMPolicy), null);
            if (storages == null || storages.Length == 0)
            {
                //AddLog("No Inventory in " + whSource.GetName()
                //    + " for " + product.GetName());
                return "-1";
            }
            //
            Decimal target = Rep.QtyToOrder;
            for (int j = 0; j < storages.Length; j++)
            {
                MStorage storage = storages[j];
                //if (storage.GetQtyOnHand().signum() <= 0)
                if (Env.Signum(storage.GetQtyOnHand()) <= 0)
                {
                    continue;
                }
                Decimal moveQty = target;
                if (storage.GetQtyOnHand().CompareTo(moveQty) < 0)
                {
                    moveQty = storage.GetQtyOnHand();
                }
                //
                VAdvantage.Model.MMovementLine line = new VAdvantage.Model.MMovementLine(moveReps);
                line.SetM_Product_ID(Rep.M_Product_ID);
                line.SetMovementQty(moveQty);
                //line.SetM_AttributeSetInstance_ID(Rep.M_AttributeSetInstance_ID);
                if (Rep.QtyToOrder.CompareTo(moveQty) != 0)
                {
                    line.SetDescription("Total: " + Rep.QtyToOrder);
                }
                line.SetQtyEntered(moveQty);
                line.Set_Value("C_UOM_ID", product.GetC_UOM_ID());

                line.SetM_Locator_ID(storage.GetM_Locator_ID());        //	from
                line.SetM_AttributeSetInstance_ID(storage.GetM_AttributeSetInstance_ID());
                line.SetM_LocatorTo_ID(M_LocatorTo_ID);                 //	to
                line.SetM_AttributeSetInstanceTo_ID(storage.GetM_AttributeSetInstance_ID());
                line.Save();
                //
                //target = target.subtract(moveQty);
                target = Decimal.Subtract(target, moveQty);
                //if (target.signum() == 0)
                if (Env.Signum(target) == 0)
                {
                    break;
                }
            }
            if (Env.Signum(target) != 0)
            {
                //AddLog("Insufficient Inventory in " + whSource.GetName()
                //    + " for " + product.GetName() + " Qty=" + target);
            }

            return moveReps.GetDocumentNo();
            //if (replenishs.Length == 0)
            //{
            //    _info = "No Source Warehouse";
            //    log.Warning(_info);
            //}
            //else
            //{
            //    _info = "#" + noMoves + info;
            //    log.Info(_info);
            //}
        }   //	createRequisition

        /// <summary>
        /// Get Locator_ID
        /// </summary>
        /// <param name="product"> product </param>
        /// <param name="wh">warehouse</param>
        /// <returns>locator with highest priority</returns>
        private int GetLocator_ID(MProduct product, MWarehouse wh)
        {
            int M_Locator_ID = MProductLocator.GetFirstM_Locator_ID(product, wh.GetM_Warehouse_ID());
            if (M_Locator_ID == 0)
            {
                M_Locator_ID = wh.GetDefaultM_Locator_ID();
            }
            return M_Locator_ID;
        }   //	getLocator_ID
        /// <summary>
        /// GetProductsAll Method
        /// </summary>
        /// <param name="ctx">ctx</param>
        /// <returns>Products All</returns>
        public List<NameIDClass> GetProductsAll(Ctx ctx)
        {
            List<NameIDClass> pInfo = new List<NameIDClass>();
            string sql = "SELECT M_Product_ID, Name FROM M_Product WHERE AD_Client_ID = " + ctx.GetAD_Client_ID() + " AND IsActive = 'Y' AND IsSummary='N'";

            sql += " ORDER BY Name";
            DataSet ds = DB.ExecuteDataset(sql);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    NameIDClass prods = new NameIDClass();
                    prods.Name = Util.GetValueOfString(ds.Tables[0].Rows[i]["Name"]);
                    prods.ID = Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_Product_ID"]);

                    pInfo.Add(prods);
                }
            }
            return pInfo;
        }
        /// <summary>
        /// SaveRepRuleAll Method
        /// </summary>
        /// <param name="Reps">Reps</param>
        /// <param name="ct">ct</param>
        /// <returns></returns>
        public string SaveRepRuleAll(List<RepRule> Reps, Ctx ct)
        {
            string res = "";
            MReplenish rep = null;

            for (int i = 0; i < Reps.Count; i++)
            {

                string sql = "SELECT * FROM  M_Replenish WHERE M_Warehouse_ID = " + Reps[i].M_Warehouse_ID + " AND M_Product_ID = " + Reps[i].M_Product_ID;
                IDataReader idr = null;
                DataTable dt = null;
                try
                {
                    idr = DB.ExecuteReader(sql, null, null);
                    dt = new DataTable();
                    dt.Load(idr);
                    idr.Close();
                    foreach (DataRow dr in dt.Rows)
                    {
                        rep = new MReplenish(ct, dr, null);
                    }
                    if (rep == null)
                    {
                        rep = new MReplenish(ct, 0, null);
                        rep.SetM_Product_ID(Reps[i].M_Product_ID);
                        rep.SetM_Warehouse_ID(Reps[i].M_Warehouse_ID);
                    }
                    rep.SetReplenishType(Reps[i].RepType);
                    rep.SetDTD001_MinOrderQty(Reps[i].Qty);
                    rep.SetDTD001_OrderPackQty(Reps[i].OrderPack);
                    rep.SetLevel_Max(Reps[i].Max);
                    rep.SetLevel_Min(Reps[i].Min);
                    if (Reps[i].SourceWarehouse != 0)
                    {
                        rep.SetM_WarehouseSource_ID(Reps[i].SourceWarehouse);
                    }
                    if (!rep.Save())
                    {

                    }

                }
                catch (Exception ex)
                {
                    if (idr != null)
                    {
                        idr.Close();
                        idr = null;
                    }
                }
            }

            return res;
        }
        /// <summary>
        /// GetModuleInfo Method
        /// </summary>
        /// <param name="_prefix">_prefix</param>
        /// <returns></returns>
        public bool GetModuleInfo(string _prefix)
        {
            bool exist = false;

            if (Env.HasModulePrefix(_prefix, out aInfo))
            {
                exist = true;
            }
            return exist;

        }
        /// <summary>
        /// GetSupplier Method
        /// </summary>
        /// <param name="ctx">ctx</param>
        /// <returns>Supplier</returns>
        public List<NameIDClass> GetSupplier(Ctx ctx)
        {
            List<NameIDClass> pInfo = new List<NameIDClass>();
            string sql = @"SELECT C_BPartner_ID, Name FROM C_BPartner WHERE AD_Client_ID = " + ctx.GetAD_Client_ID() + " AND IsActive = 'Y' AND IsVendor = 'Y'";
            sql += " ORDER BY Name";
            DataSet ds = DB.ExecuteDataset(sql);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    NameIDClass org = new NameIDClass();
                    org.Name = Util.GetValueOfString(ds.Tables[0].Rows[i]["Name"]);
                    org.ID = Util.GetValueOfInt(ds.Tables[0].Rows[i]["C_BPartner_ID"]);
                    pInfo.Add(org);
                }
            }
            return pInfo;
        }
        /// <summary>
        /// GetCart Method
        /// </summary>
        /// <param name="ctx">ctx</param>
        /// <returns>Cart value</returns>
        public List<NameIDClass> GetCart(Ctx ctx)
        {
            List<NameIDClass> pInfo = new List<NameIDClass>();
            var qry = @"SELECT VAICNT_InventoryCount_ID,VAICNT_ScanName FROM VAICNT_InventoryCount WHERE IsActive = 'Y' AND VAICNT_TransactionType = 'OT' AND AD_Client_ID = " + ctx.GetAD_Client_ID() + " ORDER BY VAICNT_ScanName";
            //qry += " ORDER BY Name";
            DataSet ds = DB.ExecuteDataset(qry);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    NameIDClass org = new NameIDClass();
                    org.Name = Util.GetValueOfString(ds.Tables[0].Rows[i]["VAICNT_ScanName"]);
                    org.ID = Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAICNT_InventoryCount_ID"]);
                    pInfo.Add(org);
                }
            }
            return pInfo;
        }
        /// <summary>
        /// GetDocType Method
        /// </summary>
        /// <param name="ctx">ctx</param>
        /// <returns>Type</returns>
        public List<DocType> GetDocType(Ctx ctx)
        {
            List<DocType> docTypes = new List<DocType>();
            var qry = @"SELECT C_DocType_ID, Name, DocBaseType, IsReleaseDocument FROM C_DocType WHERE DocBaseType IN "
                + "(SELECT Value FROM AD_Ref_List WHERE AD_Reference_ID =(SELECT AD_Reference_ID FROM AD_Reference WHERE Name = 'M_Replenishment Create')) AND IsReturnTrx ='N' AND AD_Client_ID = " + ctx.GetAD_Client_ID();
            //qry += " ORDER BY Name";
            DataSet ds = DB.ExecuteDataset(qry);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    DocType docType = new DocType();
                    docType.Name = Util.GetValueOfString(ds.Tables[0].Rows[i]["Name"]);
                    docType.DocBaseType = Util.GetValueOfString(ds.Tables[0].Rows[i]["DocBaseType"]);
                    docType.IsReleaseDocument = Util.GetValueOfString(ds.Tables[0].Rows[i]["IsReleaseDocument"]);
                    docType.ID = Util.GetValueOfInt(ds.Tables[0].Rows[i]["C_DocType_ID"]);
                    docTypes.Add(docType);
                }
            }
            return docTypes;
        }
        /// <summary>
        /// GetDocStatus Method
        /// </summary>
        /// <param name="ctx">ctx</param>
        /// <returns>DocStatus</returns>
        public List<ValueNamePair> GetDocStatus(Ctx ctx)
        {
            List<ValueNamePair> values = new List<ValueNamePair>();
            var qry = @"SELECT Value,  Name FROM AD_Ref_List WHERE ad_reference_ID = (SELECT AD_Reference_ID FROM AD_Reference  WHERE Name = '_Document Status') AND Value IN ('DR', 'CO', 'IP')";
            //qry += " ORDER BY Name";
            DataSet ds = DB.ExecuteDataset(qry);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    ValueNamePair obj = new ValueNamePair(Util.GetValueOfString(ds.Tables[0].Rows[i]["Value"]), Util.GetValueOfString(ds.Tables[0].Rows[i]["Name"]));
                    //obj.Name = Util.GetValueOfString(ds.Tables[0].Rows[i]["Name"]);
                    //obj.Key = Util.GetValueOfString(ds.Tables[0].Rows[i]["Value"]);
                    values.Add(obj);
                }
            }
            return values;
        }
        /// <summary>
        /// GetReplenishType Method
        /// </summary>
        /// <param name="ctx">ctx</param>
        /// <returns>Replenish Type</returns>
        public List<ValueNamePair> GetReplenishType(Ctx ctx)
        {
            List<ValueNamePair> pInfo = new List<ValueNamePair>();
            var qry = @"SELECT Value, Name FROM AD_Ref_List WHERE AD_Reference_ID = (SELECT AD_Reference_ID FROM AD_Reference WHERE Name = 'M_Replenish Type' ) AND Value IN ('1','2')";
            //qry += " ORDER BY Name";
            DataSet ds = DB.ExecuteDataset(qry);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    ValueNamePair obj = new ValueNamePair(Util.GetValueOfString(ds.Tables[0].Rows[i]["Value"]), Util.GetValueOfString(ds.Tables[0].Rows[i]["Name"]));
                    //obj.Name = Util.GetValueOfString(ds.Tables[0].Rows[i]["Name"]);
                    //obj.Key = Util.GetValueOfString(ds.Tables[0].Rows[i]["Value"]);
                    pInfo.Add(obj);
                }
            }
            return pInfo;
        }
        /// <summary>
        /// CreateCombo Method
        /// </summary>
        /// <param name="ctx">ctx</param>
        /// <returns>Combo</returns>
        public List<ValueNamePair> CreateCombo(Ctx ctx)
        {
            List<ValueNamePair> values = new List<ValueNamePair>();
            var qry = @"SELECT Value, Name FROM AD_Ref_List WHERE ad_reference_ID = (SELECT AD_Reference_ID FROM AD_Reference  WHERE Name = 'M_Replenishment Create')";
            //qry += " ORDER BY Name";
            DataSet ds = DB.ExecuteDataset(qry);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    ValueNamePair obj = new ValueNamePair(Util.GetValueOfString(ds.Tables[0].Rows[i]["Value"]), Util.GetValueOfString(ds.Tables[0].Rows[i]["Name"]));
                    //obj.Name = Util.GetValueOfString(ds.Tables[0].Rows[i]["Name"]);
                    //obj.Key = Util.GetValueOfString(ds.Tables[0].Rows[i]["Value"]);
                    values.Add(obj);
                }
            }
            return values;
        }
        /// <summary>
        /// GetUOM Method
        /// </summary>
        /// <param name="ctx">ctx</param>
        /// <returns>UOM</returns>
        public List<NameIDClass> GetUOM(Ctx ctx)
        {
            List<NameIDClass> objUOM = new List<NameIDClass>();
            string sql = @"SELECT C_UOM_ID,Name FROM C_UOM WHERE IsActive = 'Y'";
            sql += " ORDER BY Name";
            DataSet ds = DB.ExecuteDataset(sql);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    NameIDClass obj = new NameIDClass();
                    obj.Name = Util.GetValueOfString(ds.Tables[0].Rows[i]["Name"]);
                    obj.ID = Util.GetValueOfInt(ds.Tables[0].Rows[i]["C_UOM_ID"]);
                    objUOM.Add(obj);
                }
            }
            return objUOM;
        }
        /// <summary>
        /// LoadBindcart Method
        /// </summary>
        /// <param name="ctx">ctx</param>
        /// <param name="VAICNT_InventoryCount_ID">VAICNT_InventoryCount_ID</param>
        /// <returns>Bindcart</returns>
        public List<PriceInfo> LoadBindcart(Ctx ctx, int VAICNT_InventoryCount_ID)
        {
            List<PriceInfo> infos = new List<PriceInfo>();
            string sql = @"SELECT po.VAICNT_InventoryCountLine_ID,po.M_Product_ID,prd.Name, po.C_UOM_ID, u.Name AS UOM, po.UPC, po.M_AttributeSetInstance_ID, ats.Description, po.VAICNT_Quantity," +
                " prd.M_AttributeSet_ID FROM VAICNT_InventoryCountLine po LEFT JOIN C_UOM u ON po.C_UOM_ID = u.C_UOM_ID LEFT JOIN M_Product prd" +
                " ON po.M_Product_ID= prd.M_Product_ID LEFT JOIN M_AttributeSetInstance ats ON po.M_AttributeSetInstance_ID = ats.M_AttributeSetInstance_ID" +
                " WHERE po.VAICNT_InventoryCount_ID = " + VAICNT_InventoryCount_ID;

            DataSet ds = DB.ExecuteDataset(sql);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    PriceInfo priceInfo = new PriceInfo();
                    priceInfo.LineID = Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAICNT_InventoryCountLine_ID"]);
                    priceInfo.product_ID = Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_Product_ID"]);
                    priceInfo.Product = Util.GetValueOfString(ds.Tables[0].Rows[i]["Name"]);
                    priceInfo.C_Uom_ID = Util.GetValueOfInt(ds.Tables[0].Rows[i]["C_UOM_ID"]);
                    priceInfo.attribute_ID = Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_AttributeSetInstance_ID"]);
                    priceInfo.Attribute = Util.GetValueOfString(ds.Tables[0].Rows[i]["Description"]);
                    priceInfo.UPC = Util.GetValueOfString(ds.Tables[0].Rows[i]["UPC"]);
                    priceInfo.Qty = Util.GetValueOfDecimal(ds.Tables[0].Rows[i]["VAICNT_Quantity"]);
                    infos.Add(priceInfo);
                }
            }
            return infos;
        }
        /// <summary>
        /// LoadSubstituteGrid Method
        /// </summary>
        /// <param name="ctx">ctx</param>
        /// <param name="M_Product_ID">M_Product_ID</param>
        /// <returns>Substitute value</returns>

        public List<Substitute> LoadSubstituteGrid(Ctx ctx, int M_Product_ID)
        {
            List<Substitute> objSub = new List<Substitute>();
            string sql = @"SELECT DISTINCT p.Name as Product, p.M_Product_ID, u.Name AS UOM , (bomQtyOnHand(p.M_Product_ID,w.M_Warehouse_ID,0)) AS QtyOnHand,"
                    + " bomQtyAvailable(p.M_Product_ID,w.M_Warehouse_ID,0) AS QtyAvailable, (bomQtyReserved(p.M_Product_ID,w.M_Warehouse_ID,0))  AS QtyReserved"
                    + " FROM M_Substitute s INNER JOIN M_Product p ON (p.M_Product_ID = s.SUBSTITUTE_ID) INNER JOIN C_UOM u ON (p.C_UOM_ID = u.C_UOM_ID) LEFT OUTER JOIN M_Storage st "
                    + " ON (st.M_Product_ID = p.M_Product_ID) LEFT OUTER JOIN M_Locator l ON (st.M_Locator_ID = l.M_Locator_ID) LEFT OUTER JOIN M_Warehouse w ON (w.M_Warehouse_ID = l.M_Warehouse_ID)"
                    + " WHERE s.IsActive='Y' AND s.M_Product_ID = " + M_Product_ID;

            DataSet ds = DB.ExecuteDataset(sql);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    Substitute obj = new Substitute();
                    obj.Product = Util.GetValueOfString(ds.Tables[0].Rows[i]["product"]);
                    obj.QtyOnHand = Util.GetValueOfString(ds.Tables[0].Rows[i]["qtyonhand"]);
                    obj.UOM = Util.GetValueOfString(ds.Tables[0].Rows[i]["uom"]);
                    obj.Reserved = Util.GetValueOfString(ds.Tables[0].Rows[i]["qtyreserved"]);
                    obj.ATP = Util.GetValueOfString(ds.Tables[0].Rows[i]["qtyavailable"]);
                    obj.M_Product_ID = Util.GetValueOfInt(ds.Tables[0].Rows[i]["m_product_id"]);
                    objSub.Add(obj);
                }
            }
            return objSub;
        }
        /// <summary>
        /// LoadSuppliersRightGrid Method
        /// </summary>
        /// <param name="ctx">ctx</param>
        /// <param name="M_Product_ID">M_Product_ID</param>
        /// <returns>Suppliers value</returns>
        public List<PriceInfo> LoadSuppliersRightGrid(Ctx ctx, int M_Product_ID)
        {
            List<PriceInfo> priceInfos = new List<PriceInfo>();
            string sql = @"SELECT bp.name  AS Supplier, po.order_pack AS QtyOrderPack, u.Name AS UOM, po.order_min AS MinOrder, po.deliverytime_promised AS DeliveryTime, po.M_Product_ID"
                        + " FROM M_Product_PO po INNER JOIN C_BPartner bp  ON (bp.C_BPartner_ID = po.C_BPartner_ID) Left outer join C_UOM u on (u.C_UOM_ID = po.C_UOM_ID)"
                        + " WHERE po.IsActive = 'Y' AND po.M_Product_ID = " + M_Product_ID;

            DataSet ds = DB.ExecuteDataset(sql);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    PriceInfo obj = new PriceInfo();
                    obj.Supplier = Util.GetValueOfString(ds.Tables[0].Rows[i]["Supplier"]);
                    obj.Qty = Util.GetValueOfDecimal(ds.Tables[0].Rows[i]["QtyOrderPack"]);
                    obj.UOM = Util.GetValueOfString(ds.Tables[0].Rows[i]["UOM"]);
                    obj.OrderMin = Util.GetValueOfInt(ds.Tables[0].Rows[i]["MinOrder"]);
                    obj.DeliveryTime = Util.GetValueOfInt(ds.Tables[0].Rows[i]["DeliveryTime"]);
                    obj.product_ID = Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_Product_ID"]);
                    priceInfos.Add(obj);
                }
            }
            return priceInfos;
        }
        /// <summary>
        /// LoadKitsGrid Method
        /// </summary>
        /// <param name="ctx">ctx</param>
        /// <param name="M_ProductBOM_ID">M_ProductBOM_ID</param>
        /// <returns>Kits value</returns>
        public List<Kits> LoadKitsGrid(Ctx ctx, int M_ProductBOM_ID)
        {
            List<Kits> kits = new List<Kits>();
            string sql = @"SELECT DISTINCT p.Name as Product, u.Name as UOM, bomQtyOnHand(b.M_Product_ID,w.M_Warehouse_ID,0) AS QtyOnHand, bomQtyAvailable(b.M_Product_ID,w.M_Warehouse_ID,0) AS QtyAvailable,"
                    + " b.BOMQty AS Factor FROM M_Product_BOM b INNER JOIN M_Product p ON p.M_Product_ID = b.M_Product_ID INNER JOIN C_UOM u ON (p.C_UOM_ID = u.C_UOM_ID) "
                    + " LEFT OUTER JOIN M_Storage st ON (st.M_Product_ID = p.M_Product_ID) LEFT OUTER JOIN M_Locator l ON (st.M_Locator_ID = l.M_Locator_ID) LEFT OUTER JOIN M_Warehouse w "
                    + " ON (w.M_Warehouse_ID    = l.M_Warehouse_ID) WHERE b.IsActive='Y' AND b.M_ProductBOM_ID = " + M_ProductBOM_ID;

            DataSet ds = DB.ExecuteDataset(sql);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    Kits obj = new Kits();
                    obj.Product = Util.GetValueOfString(ds.Tables[0].Rows[i]["Product"]);
                    obj.Factor = Util.GetValueOfString(ds.Tables[0].Rows[i]["Factor"]);
                    obj.UOM = Util.GetValueOfString(ds.Tables[0].Rows[i]["UOM"]);
                    obj.QtyOnHand = Util.GetValueOfString(ds.Tables[0].Rows[i]["QtyOnHand"]);
                    obj.ATP = Util.GetValueOfString(ds.Tables[0].Rows[i]["QtyAvailable"]);
                    obj.M_Product_ID = Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_Product_ID"]);
                    kits.Add(obj);
                }
            }
            return kits;
        }
        /// <summary>
        /// LoadOrderedGrid method
        /// </summary>
        /// <param name="ctx">ctx</param>
        /// <param name="M_Product_ID">M_Product_ID</param>
        /// <param name="selWh">selWh</param>
        /// <returns></returns>
        public List<Order> LoadOrderedGrid(Ctx ctx, int M_Product_ID, List<int> selWh)
        {
            List<Order> objOrder = new List<Order>();
            StringBuilder sqlOrd = new StringBuilder("");
            var whString = "";
            for (var w = 0; w < selWh.Count; w++)
            {
                if (whString.Length > 0)
                {
                    whString = whString + ", " + selWh[w];
                }
                else
                {
                    whString = whString + selWh[w];
                }
            }
            sqlOrd.Append("SELECT ol.datepromised, o.dateordered, ol.m_product_ID, ol.qtyordered, ol.qtyentered, ol.qtyreserved, bp.name as supplier FROM c_order o INNER JOIN c_Orderline ol ON (ol.c_ORder_ID = o.C_Order_ID) "
                    + " inner join c_bpartner bp on (bp.c_BPartner_ID = o.C_Bpartner_ID) WHERE o.IsSOTrx = 'N' AND o.IsReturnTrx = 'N' AND ol.QtyReserved > 0 AND o.DocStatus IN ('CO', 'CL') AND ol.M_Product_ID = " + M_Product_ID);
            if (whString.Length > 0)
            {
                sqlOrd.Append(" AND o.M_Warehouse_ID IN (" + whString + ")");
            }

            sqlOrd.Append(" ORDER BY o.Created DESC");
            DataSet ds = DB.ExecuteDataset(sqlOrd.ToString());
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    Order obj = new Order();
                    obj.DatePromised = Util.GetValueOfDateTime(ds.Tables[0].Rows[i]["datepromised"]).Value.ToShortDateString();
                    obj.Quantity = Util.GetValueOfString(ds.Tables[0].Rows[i]["qtyordered"]);
                    obj.DateOrdered = Util.GetValueOfDateTime(ds.Tables[0].Rows[i]["dateordered"]).Value.ToShortDateString();
                    obj.Supplier = Util.GetValueOfString(ds.Tables[0].Rows[i]["supplier"]);
                    obj.QtyReserved = Util.GetValueOfString(ds.Tables[0].Rows[i]["qtyreserved"]);
                    objOrder.Add(obj);
                }
            }
            return objOrder;
        }
        /// <summary>
        /// TransactionsGrid Method
        /// </summary>
        /// <param name="ctx">ctx</param>
        /// <param name="M_Product_ID">M_Product_ID</param>
        /// <param name="selWh">selWh</param>
        /// <returns>Selected value</returns>
        public List<Transaction> LoadTransactionsGrid(Ctx ctx, int M_Product_ID, List<int> selWh)
        {
            List<Transaction> objTran = new List<Transaction>();
            StringBuilder sqlTrx = new StringBuilder("");

            sqlTrx.Append("SELECT t.M_Product_ID, t.M_AttributeSetInstance_ID, asi.description, t.MOVEMENTQTY , t.MOVEMENTTYPE, t.CurrentQty, t.MovementDate, lc.Value AS Locator, t.M_Transaction_ID, iol.M_InOutLine_ID, ivl.M_InventoryLine_ID, mvl.M_MovementLine_ID, "
                   + " Case when t.MovementQty > 0 Then t.movementQty else 0 end as InventoryIn, Case when t.MovementQty < 0 Then t.movementQty else 0 end as InventoryOut, "
                   + " CASE WHEN NVL(iol.M_InOutLine_ID,0) > 0 THEN iol.M_InoutLine_ID WHEN NVL(ivl.M_InventoryLine_ID,0) > 0 THEN ivl.M_InventoryLine_ID WHEN NVL(mvl.M_MovementLine_ID,0) > 0 THEN mvl.M_MovementLine_ID END AS ID, "
                   + " CASE WHEN NVL(iol.M_InOutLine_ID,0) > 0 THEN io.DocumentNo WHEN NVL(ivl.M_InventoryLine_ID,0) > 0 THEN iv.DocumentNo WHEN NVL(mvl.M_MovementLine_ID,0) > 0 THEN mv.DocumentNo END AS DocumentNo,"
                   + " CASE WHEN NVL(iol.M_InOutLine_ID,0) > 0 THEN dtio.Name WHEN NVL(ivl.M_InventoryLine_ID,0) > 0 THEN dtiv.Name WHEN NVL(mvl.M_MovementLine_ID,0) > 0 THEN dtmv.Name END AS DocType "
                   + " FROM M_Transaction t LEFT OUTER JOIN M_InoutLine iol ON (iol.M_InOutLine_ID = t.M_InOutLine_ID) LEFT OUTER JOIN M_Inout io ON (iol.M_InOut_ID = io.M_InOut_ID) LEFT OUTER JOIN C_DocType dtio "
                   + " ON (dtio.C_DocType_ID = io.C_DocType_ID) LEFT OUTER JOIN M_inventoryLine ivl ON (ivl.M_inventoryLine_ID = t.M_inventoryLine_ID) LEFT OUTER JOIN M_Inventory iv ON (ivl.M_Inventory_ID = iv.M_Inventory_ID) "
                   + " LEFT OUTER JOIN C_DocType dtiv ON (dtiv.C_DocType_ID = iv.C_DocType_ID) LEFT OUTER JOIN M_MovementLine mvl ON (mvl.M_MovementLine_ID = t.M_MovementLine_ID) LEFT OUTER JOIN M_Movement mv "
                   + " ON (mvl.M_Movement_ID = mv.M_Movement_ID) LEFT OUTER JOIN C_DocType dtmv ON (dtmv.C_DocType_ID = mv.C_DocType_ID) LEFT OUTER JOIN M_AttributeSetInstance asi  ON (t.M_AttributeSetInstance_ID = asi.M_AttributeSetInstance_ID) LEFT OUTER JOIN M_Locator lc ON (lc.M_Locator_ID = t.M_Locator_ID) WHERE t.M_Product_ID = " + M_Product_ID);
            if (selWh.Count > 0)
            {
                var whString = "";
                for (var w = 0; w < selWh.Count; w++)
                {
                    if (whString.Length > 0)
                    {
                        whString = whString + ", " + selWh[w];
                    }
                    else
                    {
                        whString = whString + selWh[w];
                    }
                }
                sqlTrx.Append(" AND t.M_Locator_ID IN (Select loc.M_Locator_ID from m_warehouse wh inner join m_locator loc on (loc.m_warehouse_ID = wh.M_Warehouse_ID) where wh.m_warehouse_ID in (" + whString + ")) ");
            }

            sqlTrx.Append(" ORDER BY t.MovementDate DESC");


            DataSet ds = DB.ExecuteDataset(sqlTrx.ToString());
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    Transaction obj = new Transaction();
                    obj.DocumentType = Util.GetValueOfString(ds.Tables[0].Rows[i]["DocType"]);
                    obj.DocumentNo = Util.GetValueOfString(ds.Tables[0].Rows[i]["DocumentNo"]);
                    obj.Locator = Util.GetValueOfString(ds.Tables[0].Rows[i]["Locator"]);
                    obj.Date = Util.GetValueOfDateTime(ds.Tables[0].Rows[i]["MovementDate"]).Value.ToShortDateString();
                    obj.InventoryIn = Util.GetValueOfString(ds.Tables[0].Rows[i]["InventoryIn"]);
                    obj.InventoryOut = Util.GetValueOfString(ds.Tables[0].Rows[i]["InventoryOut"]);
                    obj.Attribute = Util.GetValueOfString(ds.Tables[0].Rows[i]["description"]);
                    obj.Balance = Util.GetValueOfString(ds.Tables[0].Rows[i]["CurrentQty"]);
                    objTran.Add(obj);
                }
            }
            return objTran;
        }
        /// <summary>
        /// VariantGrid Method
        /// </summary>
        /// <param name="ctx">ctx</param>
        /// <param name="M_Product_ID">M_Product_ID</param>
        /// <param name="selWh">selWh</param>
        /// <param name="orgString">orgString</param>
        /// <returns>selected value</returns>
        public List<Variant> LoadVariantGrid(Ctx ctx, int M_Product_ID, List<int> selWh, string orgString)
        {
            List<Variant> objVariant = new List<Variant>();
            StringBuilder sqlVar = new StringBuilder("");

            sqlVar.Append(@"SELECT Name,  M_Product_ID,  UOM,  lot,  serno,  guaranteedate,  SUM(QtyOnHand) AS QtyOnHand,  M_AttributeSetInstance_ID,  Description FROM  ( ");
            if (selWh != null && selWh.Count > 0)
            {
                //var whString = "";
                for (var w = 0; w < selWh.Count; w++)
                {
                    if (w == 0)
                    {
                        sqlVar.Append(" SELECT distinct p.Name, p.M_Product_ID, u.Name as UOM, ats.lot, ats.serno, ats.guaranteedate, bomQtyOnHandAttr(p.M_Product_ID, s.M_AttributeSetInstance_ID ,w.M_Warehouse_ID,0) AS QtyOnHand, "
                            + " s.M_AttributeSetInstance_ID, ats.Description FROM M_Storage s INNER JOIN M_AttributeSetInstance ats ON (s.M_AttributeSetInstance_ID = ats.M_AttributeSetInstance_ID) "
                            + " INNER JOIN M_LOcator l ON (l.M_Locator_ID = s.M_Locator_ID) INNER JOIN M_Warehouse w ON (w.M_Warehouse_ID = l.M_Warehouse_ID) "
                            + " INNER JOIN M_Product p ON (p.M_Product_ID = s.M_Product_ID) INNER JOIN C_UOM u ON (p.C_UOM_ID = u.C_UOM_ID) WHERE w.M_Warehouse_ID = " + selWh[w] + " AND s.M_Product_ID = " + M_Product_ID);
                        //+ " AND bomQtyOnHandAttr(p.M_Product_ID, s.M_AttributeSetInstance_ID ,w.M_Warehouse_ID,0) > 0";
                    }
                    else
                    {
                        sqlVar.Append("  UNION SELECT distinct p.Name, p.M_Product_ID, u.Name as UOM, ats.lot, ats.serno, ats.guaranteedate, bomQtyOnHandAttr(p.M_Product_ID, s.M_AttributeSetInstance_ID ,w.M_Warehouse_ID,0) AS QtyOnHand, "
                            + " s.M_AttributeSetInstance_ID, ats.Description FROM M_Storage s INNER JOIN M_AttributeSetInstance ats ON (s.M_AttributeSetInstance_ID = ats.M_AttributeSetInstance_ID) "
                            + " INNER JOIN M_LOcator l ON (l.M_Locator_ID = s.M_Locator_ID) INNER JOIN M_Warehouse w ON (w.M_Warehouse_ID = l.M_Warehouse_ID) "
                            + " INNER JOIN M_Product p ON (p.M_Product_ID = s.M_Product_ID) INNER JOIN C_UOM u ON (p.C_UOM_ID = u.C_UOM_ID) WHERE w.M_Warehouse_ID = " + selWh[w] + " AND s.M_Product_ID = " + M_Product_ID);
                        // + " AND bomQtyOnHandAttr(p.M_Product_ID, s.M_AttributeSetInstance_ID ,w.M_Warehouse_ID,0) > 0";
                    }
                    if (orgString.Length > 0)
                        sqlVar.Append(" AND w.AD_Org_ID IN (" + orgString + ")");
                    sqlVar.Append(" ) t GROUP BY Name,  M_Product_ID,  UOM,  lot,  serno,  M_AttributeSetInstance_ID,  guaranteedate,  Description");
                }
            }
            else
            {
                sqlVar.Append("SELECT distinct p.Name, p.M_Product_ID, u.Name as UOM, ats.lot, ats.serno, ats.guaranteedate, bomQtyOnHandAttr(p.M_Product_ID, s.M_AttributeSetInstance_ID ,w.M_Warehouse_ID,0) AS QtyOnHand, "
                    + " s.M_AttributeSetInstance_ID, ats.Description FROM M_Storage s INNER JOIN M_AttributeSetInstance ats ON (s.M_AttributeSetInstance_ID = ats.M_AttributeSetInstance_ID) "
                    + " INNER JOIN M_LOcator l ON (l.M_Locator_ID = s.M_Locator_ID) INNER JOIN M_Warehouse w ON (w.M_Warehouse_ID = l.M_Warehouse_ID) "
                    + " INNER JOIN M_Product p ON (p.M_Product_ID = s.M_Product_ID) INNER JOIN C_UOM u ON (p.C_UOM_ID = u.C_UOM_ID) WHERE s.M_Product_ID = " + M_Product_ID);
                // + " AND bomQtyOnHandAttr(p.M_Product_ID, s.M_AttributeSetInstance_ID ,w.M_Warehouse_ID,0) > 0";


                if (orgString.Length > 0)
                    sqlVar.Append(" AND w.AD_Org_ID IN (" + orgString + ")");
                sqlVar.Append(") t GROUP BY Name,  M_Product_ID,  UOM,  lot,  serno,  M_AttributeSetInstance_ID,  guaranteedate,  Description");
            }

            DataSet ds = DB.ExecuteDataset(sqlVar.ToString());
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    Variant obj = new Variant();
                    obj.Attribute = Util.GetValueOfString(ds.Tables[0].Rows[i]["Description"]);
                    obj.QtyOnHand = Util.GetValueOfString(ds.Tables[0].Rows[i]["QtyOnHand"]);
                    obj.UOM = Util.GetValueOfString(ds.Tables[0].Rows[i]["UOM"]);
                    obj.SerialNo = Util.GetValueOfString(ds.Tables[0].Rows[i]["serno"]);
                    obj.LotNo = Util.GetValueOfString(ds.Tables[0].Rows[i]["lot"]);
                    obj.ExpDate = Util.GetValueOfString(ds.Tables[0].Rows[i]["guaranteedate"]);
                    obj.M_Product_ID = Util.GetValueOfInt(ds.Tables[0].Rows[i]["m_product_id"]);
                    objVariant.Add(obj);
                }
            }
            return objVariant;
        }
        /// <summary>
        /// LocatorGrid method
        /// </summary>
        /// <param name="ctx">ctx</param>
        /// <param name="M_Product_ID">M_Product_ID</param>
        /// <param name="selWh">selWh</param>
        /// <param name="orgString">orgString</param>
        /// <returns></returns>
        public List<Transaction> LocatorGrid(Ctx ctx, int M_Product_ID, List<int> selWh, string orgString)
        {
            List<Transaction> objTran = new List<Transaction>();

            var selQuery = "SELECT p.Name, p.M_Product_ID, l.M_Locator_ID, p.Value, w.Name as Warehouse, l.Value as Locator, s.M_AttributeSetInstance_ID, asi.Description, (SELECT MAX(io.DateAcct) FROM M_InoutLine iol "
                 + " INNER JOIN M_Inout io ON (io.M_Inout_ID      = iol.M_Inout_ID) WHERE iol.M_Product_ID = p.M_Product_ID  AND io.IsSOTrx = 'N' AND iol.M_Locator_ID = l.M_Locator_ID) as LastReceipt, "
                 + " (SELECT NVL(SUM(lc.targetqty),0) FROM m_inoutlineconfirm lc INNER JOIN m_inoutconfirm ioc ON (ioc.M_InoutConfirm_ID = lc.M_InoutConfirm_ID) INNER JOIN m_inoutline iol ON (iol.M_Inoutline_ID = lc.m_inoutLine_ID) INNER JOIN m_inout io ON (iol.M_Inout_ID = io.m_inout_ID) "
                 + " WHERE ioc.docstatus NOT IN  ('CO', 'CL')  AND io.IsSOTrx = 'N'  AND iol.M_Locator_ID  = l.M_Locator_ID) as QtyUnconfirmed, "
                 + " SUM(s.QtyOnHand) AS QtyOnHand "
                 + " FROM M_Product p INNER JOIN M_Storage s  ON (s.M_Product_ID = p.M_Product_ID) INNER JOIN M_Locator l ON (l.M_Locator_ID = s.M_Locator_ID) INNER JOIN M_Warehouse w "
                 + " ON (l.M_Warehouse_ID = w.M_Warehouse_ID) LEFT OUTER JOIN M_AttributeSetInstance asi  ON (s.M_AttributeSetInstance_ID = asi.M_AttributeSetInstance_ID) WHERE p.M_Product_ID = " + M_Product_ID + " AND p.AD_Client_ID = " + ctx.GetAD_Client_ID();
            var groupBySec = " GROUP BY p.Name, p.M_Product_ID, l.M_Locator_ID, p.Value, w.Name, l.Value, s.M_AttributeSetInstance_ID, asi.Description, "
                + " w.M_Warehouse_ID  ";
            selQuery = selQuery + groupBySec;

            DataSet ds = DB.ExecuteDataset(selQuery);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    Transaction obj = new Transaction();
                    obj.Warehouse = Util.GetValueOfString(ds.Tables[0].Rows[i]["Warehouse"]);
                    obj.Locator = Util.GetValueOfString(ds.Tables[0].Rows[i]["Locator"]);
                    obj.Quantity = Util.GetValueOfString(ds.Tables[0].Rows[i]["QtyOnHand"]);
                    obj.Unconfirmed = Util.GetValueOfString(ds.Tables[0].Rows[i]["QtyUnconfirmed"]);
                    obj.Attribute = Util.GetValueOfString(ds.Tables[0].Rows[i]["description"]);
                    if (Util.GetValueOfDateTime(ds.Tables[0].Rows[i]["LastReceipt"]) != null)
                    {
                        obj.LastReceipt = Util.GetValueOfDateTime(ds.Tables[0].Rows[i]["LastReceipt"]).Value.ToShortDateString();
                    }
                    obj.M_Product_ID = Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_Product_ID"]);
                    objTran.Add(obj);
                }
            }
            return objTran;
        }
        /// <summary>
        /// LoadDemandGrid Method
        /// </summary>
        /// <param name="ctx">ctx</param>
        /// <param name="M_Product_ID">M_Product_ID</param>
        /// <param name="selWh">selWh</param>
        /// <param name="orgString">orgString</param>
        /// <returns>select value</returns>
        public List<Transaction> LoadDemandGrid(Ctx ctx, int M_Product_ID, List<int> selWh, string orgString)
        {
            List<Transaction> objTran = new List<Transaction>();
            StringBuilder sqlDmd = new StringBuilder("");

            sqlDmd.Append("SELECT * FROM ( SELECT o.Created, o.documentno, ol.QtyReserved AS qtyentered, dt.name as DocType, o.datepromised, bp.name as demandedby FROM C_Order o INNER JOIN c_orderline ol ON (ol.C_Order_ID = o.C_Order_ID) "
                   + " INNER JOIN C_DocType dt ON (o.C_DocTypeTarget_ID = dt.C_DocType_ID) INNER JOIN C_BPartner bp ON (bp.C_BPartner_ID = o.C_BPartner_ID) WHERE o.IsSOTrx = 'Y' AND o.IsReturnTrx = 'N' AND o.DocStatus IN ('CO', 'CL') AND ol.QtyReserved >0 AND ol.M_Product_ID = " + M_Product_ID);

            var selOrgs = "";
            if (selOrgs.Length > 0)
            {
                for (var w = 0; w < selOrgs.Length; w++)
                {
                    if (orgString.Length > 0)
                    {
                        orgString = orgString + ", " + selOrgs[w];
                    }
                    else
                    {
                        orgString += "0, ";
                        orgString += selOrgs[w];
                    }
                }
                sqlDmd.Append(" AND o.AD_Org_ID IN (0, " + orgString + ")");
            }

            var whString = "";
            if (selWh.Count > 0)
            {
                for (var w = 0; w < selWh.Count; w++)
                {
                    if (whString.Length > 0)
                    {
                        whString = whString + ", " + selWh[w];
                    }
                    else
                    {
                        whString = whString + selWh[w];
                    }
                }
                sqlDmd.Append(" AND o.M_Warehouse_ID IN (" + whString + ")");
            }

            sqlDmd.Append(" UNION SELECT r.Created, r.DocumentNo, rl.DTD001_ReservedQty as qtyentered, dt.name AS DocType, r.daterequired as datepromised, w.name AS demandedby  FROM m_requisitionline rl "
                + " INNER JOIN M_Requisition r ON (r.M_Requisition_ID = rl.M_Requisition_ID) INNER JOIN C_DocType dt ON (r.C_DocType_ID = dt.C_DocType_ID) INNER JOIN M_Warehouse w "
                + " ON (r.M_Warehouse_ID = w.M_Warehouse_ID) INNER JOIN m_product p ON (p.M_Product_ID = rl.M_Product_ID) WHERE r.DocStatus IN ('CO', 'CL') AND rl.DTD001_ReservedQty > 0 AND r.IsActive = 'Y' AND rl.M_Product_ID = " + M_Product_ID);
            if (orgString.Length > 0)
            {
                sqlDmd.Append(" AND r.AD_Org_ID IN (0, " + orgString + ")");
            }

            if (whString.Length > 0)
            {
                sqlDmd.Append(" AND r.M_Warehouse_ID IN (0, " + whString + ")");
            }

            if (Env.IsModuleInstalled("VAMFG_"))
            {
                sqlDmd.Append(" UNION SELECT wo.Created, wo.documentno, wo.vamfg_qtyentered AS qtyentered, dt.Name AS doctype, wo.VAMFG_DateScheduleTo as datepromised, bp.name as demandedby FROM vamfg_M_workorder wo "
                    + " INNER JOIN c_doctype dt ON (dt.C_DocType_ID = wo.C_DocType_ID) LEFT OUTER JOIN C_BPartner bp ON (bp.C_BPartner_ID = wo.C_BPartner_ID) INNER JOIN m_product p "
                    + " ON (p.M_Product_ID = wo.M_Product_ID) WHERE wo.DocStatus IN ('CO', 'CL') AND wo.IsActive   = 'Y' AND wo.M_Product_ID = " + M_Product_ID);
                if (orgString.Length > 0)
                {
                    sqlDmd.Append(" AND wo.AD_Org_ID IN (0, " + orgString + ")");
                }

                if (whString.Length > 0)
                {
                    sqlDmd.Append(" AND wo.M_Warehouse_ID IN (0, " + whString + ")");
                }
            }

            sqlDmd.Append(") ORDER BY Created DESC");

            DataSet ds = DB.ExecuteDataset(sqlDmd.ToString());
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    Transaction obj = new Transaction();
                    obj.DocumentType = Util.GetValueOfString(ds.Tables[0].Rows[i]["DocType"]);
                    obj.DocumentNo = Util.GetValueOfString(ds.Tables[0].Rows[i]["DocumentNo"]);
                    obj.Quantity = Util.GetValueOfString(ds.Tables[0].Rows[i]["qtyentered"]);
                    obj.DatePromised = Util.GetValueOfDateTime(ds.Tables[0].Rows[i]["datepromised"]).Value.ToShortDateString();
                    obj.DemandedBy = Util.GetValueOfString(ds.Tables[0].Rows[i]["demandedby"]);
                    obj.AvailabilityStatus = "";
                    obj.M_Product_ID = 0;
                    objTran.Add(obj);
                }
            }
            return objTran;
        }
        /// <summary>
        /// ReplenishmentPopGrid Method
        /// </summary>
        /// <param name="ctx">ctx</param>
        /// <param name="M_Warehouse_ID">M_Warehouse_ID</param>
        /// <param name="sqlWhere">sqlWhere</param>
        /// <returns>Value in pop</returns>
        public List<RepRule> LoadReplenishmentPopGrid(Ctx ctx, int M_Warehouse_ID, string sqlWhere)
        {
            List<RepRule> repRules = new List<RepRule>();
            StringBuilder sqlRep = new StringBuilder("");
            if (Env.IsModuleInstalled("DTD001_"))
            {
                sqlRep.Append("SELECT p.Name AS Product, w.Name      AS Warehouse, w.M_Warehouse_ID, NVL(rep.Level_Max,0) AS Maxi, NVL(rep.Level_Min,0) AS Mini, rep.ReplenishType AS rtype, "
                    + " NVL(DTD001_MinOrderQty,0)       AS Qty,  NVL(DTD001_OrderPackQty,0)      AS OrderPack,  rep.M_WarehouseSource_ID AS SourceWarehouse,  p.M_Product_ID FROM M_Product p "
                    + " Left Outer Join M_Replenish rep ON (p.M_Product_ID = rep.M_Product_ID) Left JOIN M_Warehouse w ON (w.M_Warehouse_ID = rep.M_Warehouse_ID) LEFT JOIN M_Warehouse w1 "
                    + " ON (w1.M_Warehouse_ID   = rep.M_WarehouseSource_ID) WHERE w.M_Warehouse_ID = " + M_Warehouse_ID + " AND p.IsActive      = 'Y' AND p.AD_Client_ID        = " + ctx.GetAD_Client_ID());
            }
            else
            {
                sqlRep.Append("SELECT p.Name as Product, w.Name AS Warehouse, w.M_Warehouse_ID, NVL(rep.Level_Max,0) AS Maxi, NVL(rep.Level_Min,0) AS Mini, rep.ReplenishType AS rtype, 0 AS Qty, 0 AS OrderPack,"
                    + " rep.M_WarehouseSource_ID AS SourceWarehouse, p.M_Product_ID FROM M_Product p LEFT JOIN M_Replenish rep ON (p.M_Product_ID = rep.M_Product_ID) LEFT JOIN M_Warehouse w "
                    + " ON (w.M_Warehouse_ID = rep.M_Warehouse_ID) LEFT JOIN M_Warehouse w1 ON (w1.M_Warehouse_ID = rep.M_WarehouseSource_ID) WHERE w.M_Warehouse_ID = " + M_Warehouse_ID + " AND p.IsActive = 'Y' AND p.AD_Client_ID = " + ctx.GetAD_Client_ID());
            }
            sqlRep.Append(" AND p.M_Product_ID IN (" + sqlWhere + "");

            DataSet ds = DB.ExecuteDataset(sqlRep.ToString());
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    RepRule obj = new RepRule();
                    obj.Product = Util.GetValueOfString(ds.Tables[0].Rows[i]["Product"]);
                    obj.Warehouse = Util.GetValueOfString(ds.Tables[0].Rows[i]["Warehouse"]);
                    obj.RepType = Util.GetValueOfString(ds.Tables[0].Rows[i]["rtype"]);
                    obj.Min = Util.GetValueOfDecimal(ds.Tables[0].Rows[i]["Mini"]);
                    obj.Max = Util.GetValueOfDecimal(ds.Tables[0].Rows[i]["Maxi"]);
                    obj.Qty = Util.GetValueOfDecimal(ds.Tables[0].Rows[i]["Qty"]);
                    obj.OrderPack = Util.GetValueOfDecimal(ds.Tables[0].Rows[i]["OrderPack"]);
                    obj.SourceWarehouse = Util.GetValueOfInt(ds.Tables[0].Rows[i]["SourceWarehouse"]);
                    obj.M_Warehouse_ID = Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_Warehouse_ID"]);
                    obj.M_Product_ID = Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_Product_ID"]);
                    repRules.Add(obj);
                }
            }
            return repRules;
        }
        /// <summary>
        /// ReplenishmentBGrid Method
        /// </summary>
        /// <param name="ct">ct</param>
        /// <param name="M_Product_ID">M_Product_ID</param>
        /// <param name="selWh">selWh</param>
        /// <returns></returns>
        public List<RepRule> LoadReplenishmentBGrid(Ctx ct, int M_Product_ID, List<int> selWh)
        {
            //DTD001_
            List<RepRule> repRules = new List<RepRule>();
            StringBuilder sqlRep = new StringBuilder();

            if (Env.IsModuleInstalled("DTD001_"))
            {
                sqlRep.Append( "SELECT w.Name AS Warehouse, w.M_Warehouse_ID, rep.Level_Max AS Maxi, rep.Level_Min AS Mini, rep.ReplenishType AS rtype, DTD001_MinOrderQty AS Qty, DTD001_OrderPackQty AS OrderPack,"
                    + " rep.M_WarehouseSource_ID AS SourceWarehouse, p.M_Product_ID FROM M_Replenish rep INNER JOIN M_Product p ON (p.M_Product_ID = rep.M_Product_ID) INNER JOIN M_Warehouse w "
                    + " ON (w.M_Warehouse_ID = rep.M_Warehouse_ID) LEFT JOIN M_Warehouse w1 ON (w1.M_Warehouse_ID = rep.M_WarehouseSource_ID) WHERE rep.IsActive = 'Y' AND p.M_Product_ID    = " + M_Product_ID);
            }
            else
            {
                sqlRep.Append( "SELECT w.Name AS Warehouse, w.M_Warehouse_ID, rep.Level_Max AS Maxi, rep.Level_Min AS Mini, rep.ReplenishType AS rtype, 0 AS Qty, 0 AS OrderPack,"
                    + " rep.M_WarehouseSource_ID AS SourceWarehouse, p.M_Product_ID FROM M_Replenish rep INNER JOIN M_Product p ON (p.M_Product_ID = rep.M_Product_ID) INNER JOIN M_Warehouse w "
                    + " ON (w.M_Warehouse_ID = rep.M_Warehouse_ID) LEFT JOIN M_Warehouse w1 ON (w1.M_Warehouse_ID = rep.M_WarehouseSource_ID) WHERE rep.IsActive = 'Y' AND p.M_Product_ID    = " + M_Product_ID);
            }

            if (selWh != null && selWh.Count > 0)
            {
                var whString = "";
                for (var w = 0; w < selWh.Count; w++)
                {
                    if (whString.Length > 0)
                    {
                        whString = whString + ", " + selWh[w];
                    }
                    else
                    {
                        whString = whString + selWh[w];
                    }
                }
                sqlRep.Append( " AND w.M_Warehouse_ID IN (" + whString + ")");
            }

            DataSet ds = DB.ExecuteDataset(sqlRep.ToString());
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    RepRule obj = new RepRule();
                    obj.Warehouse = Util.GetValueOfString(ds.Tables[0].Rows[i]["Warehouse"]);
                    obj.RepType = Util.GetValueOfString(ds.Tables[0].Rows[i]["rtype"]);
                    obj.Min = Util.GetValueOfDecimal(ds.Tables[0].Rows[i]["Mini"]);
                    obj.Max = Util.GetValueOfDecimal(ds.Tables[0].Rows[i]["Maxi"]);
                    obj.Qty = Util.GetValueOfDecimal(ds.Tables[0].Rows[i]["Qty"]);
                    obj.OrderPack = Util.GetValueOfDecimal(ds.Tables[0].Rows[i]["OrderPack"]);
                    obj.SourceWarehouse = Util.GetValueOfInt(ds.Tables[0].Rows[i]["SourceWarehouse"]);
                    obj.M_Warehouse_ID = Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_Warehouse_ID"]);
                    obj.M_Product_ID = Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_Product_ID"]);
                    repRules.Add(obj);
                }
            }
            return repRules;
        }
        /// <summary>
        /// Zoom Method
        /// </summary>
        /// <param name="windowName">windowName</param>
        /// <returns>window</returns>
        public int LoadWindow(string windowName)
        {
            string sql = "SELECT ad_window_id FROM ad_window WHERE name = '" + windowName + "'";
            int rule = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));
            return rule;
        }
    }

    public class Variant
    {
        public string Attribute { get; set; }
        public string QtyOnHand { get; set; }
        public string UOM { get; set; }
        public string SerialNo { get; set; }
        public string LotNo { get; set; }
        public string ExpDate { get; set; }
        public int M_Product_ID { get; set; }

    }
    public class Transaction
    {
        public string DocumentType { get; set; }
        public string DocumentNo { get; set; }
        public string Locator { get; set; }
        public string Date { get; set; }
        public string InventoryIn { get; set; }
        public string InventoryOut { get; set; }
        public string Attribute { get; set; }
        public string Balance { get; set; }
        public string Warehouse { get; set; }
        public string Quantity { get; set; }
        public string Unconfirmed { get; set; }
        public string LastReceipt { get; set; }
        public string DemandedBy { get; set; }
        public string DatePromised { get; set; }
        public string AvailabilityStatus { get; set; }
        public int M_Product_ID { get; set; }

    }
    public class Order
    {
        public string DatePromised { get; set; }
        public string Quantity { get; set; }
        public string DateOrdered { get; set; }
        public string Supplier { get; set; }
        public string QtyReserved { get; set; }
    }

    public class Kits
    {
        public string Product { get; set; }
        public string Factor { get; set; }
        public string UOM { get; set; }
        public string QtyOnHand { get; set; }
        public string ATP { get; set; }
        public int M_Product_ID { get; set; }
    }

    public class ProdCatInfo
    {
        public string Catname { get; set; }
        public int ProdCount { get; set; }
        public int M_ProdCatID { get; set; }
    }

    public class NameIDClass
    {
        public string Name { get; set; }
        public int ID { get; set; }
    }

    public class DocType
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string DocBaseType { get; set; }
        public string IsReleaseDocument { get; set; }
    }

    public class Substitute
    {
        public string Product { get; set; }
        public string QtyOnHand { get; set; }
        public string UOM { get; set; }
        public string Reserved { get; set; }
        public string ATP { get; set; }
        public int M_Product_ID { get; set; }
    }
    public class PriceInfo
    {
        public int recid { get; set; }
        public int LineID { get; set; }
        public int product_ID { get; set; }
        public string Product { get; set; }
        public decimal PriceList { get; set; }
        public decimal PriceStd { get; set; }
        public decimal PriceLimit { get; set; }
        public int C_Uom_ID { get; set; }
        public string UOM { get; set; }
        public string Lot { get; set; }
        public string UPC { get; set; }
        public decimal Qty { get; set; }
        public int attribute_ID { get; set; }
        public string Attribute { get; set; }
        public bool updated { get; set; }
        public decimal OrderMin { get; set; }
        public decimal OrderPack { get; set; }
        public int C_Currency_ID { get; set; }
        public int DeliveryTime { get; set; }
        public string Supplier { get; set; }
    }

    public class RepCreateData
    {
        public int AD_Client_ID { get; set; }
        public int AD_Org_ID { get; set; }
        public string RepCreate { get; set; }
        public int M_PriceList_ID { get; set; }
        public int M_WarehouseSource_ID { get; set; }
        public int M_Product_ID { get; set; }
        public int M_Warehouse_ID { get; set; }
        public int C_BPartner_ID { get; set; }
        public int C_DocType_ID { get; set; }
        public int M_AttributeSetInstance_ID { get; set; }
        public string DocStatus { get; set; }
        public string ReplenishmentType { get; set; }
        public decimal Max { get; set; }
        public decimal Min { get; set; }
        public decimal QtyOnHand { get; set; }
        public decimal Ordered { get; set; }
        public decimal ReqReserved { get; set; }
        public decimal Reserved { get; set; }
        public decimal QtyToOrder { get; set; }
    }

    public class RepRule
    {
        public string Product { get; set; }
        public int M_Product_ID { get; set; }
        public int M_Warehouse_ID { get; set; }
        public string RepType { get; set; }
        public decimal Max { get; set; }
        public decimal Min { get; set; }
        public decimal Qty { get; set; }
        public decimal OrderPack { get; set; }
        public int SourceWarehouse { get; set; }
        public string Warehouse { get; set; }
    }

    public class RepGet
    {
        public int RequisitionNo { get; set; }
        public string Date { get; set; }
        public int QtyDemanded { get; set; }
        public int QtyReceived { get; set; }
        public int QtyPending { get; set; }
        public int M_Product_ID { get; set; }

    }
}