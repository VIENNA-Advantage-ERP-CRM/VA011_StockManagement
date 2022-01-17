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
        /// 
        /// </summary>
        /// <param name="pageNo"></param>
        /// <param name="pageSize"></param>
        /// <param name="ctx"></param>
        /// <returns></returns>
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
        /// 
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
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
        /// 
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
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

        public List<NameIDClass> GetSupplier(Ctx ctx, string value, bool fill)
        {
            List<NameIDClass> pInfo = new List<NameIDClass>();
            string sql = @"SELECT C_BPartner_ID, Name FROM C_BPartner WHERE AD_Client_ID = " + ctx.GetAD_Client_ID() + " AND IsActive = 'Y' AND IsVendor = 'Y'";
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
                    org.ID = Util.GetValueOfInt(ds.Tables[0].Rows[i]["C_BPartner_ID"]);
                    pInfo.Add(org);
                }
            }
            return pInfo;
        }

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
                            sbSql.Append("SELECT SUM (QtyAvailable) AS QtyAvailable, SUM( QtyOnHand) AS QtyOnHand, SUM( QtyReserved) AS QtyReserved,  SUM(QtyOrdered) AS QtyOrdered FROM ("
                            + "SELECT bomQtyAvailableAttr(p.M_Product_ID,s.M_AttributeSetInstance_ID,w.M_Warehouse_ID,0) AS QtyAvailable,"
                            + " bomQtyOnHandAttr(p.M_Product_ID,s.M_AttributeSetInstance_ID,w.M_Warehouse_ID,0) AS QtyOnHand,"
                            + " bomQtyReservedAttr(p.M_Product_ID,s.M_AttributeSetInstance_ID,w.M_Warehouse_ID,0) AS QtyReserved,"
                            + " bomQtyOrderedAttr(p.M_Product_ID,s.M_AttributeSetInstance_ID,w.M_Warehouse_ID,0) AS QtyOrdered,l.M_Warehouse_ID");
                            sbSql.Append(" FROM M_Product p left join M_Storage s ON p.M_Product_ID = s.M_Product_ID  INNER JOIN M_Locator l "
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

        public DataSet GetReplenishments(List<int> Warehouses, int M_Product_ID, Ctx ct)
        {
            DataSet dsRep = new DataSet();
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
                        dsRep.Tables[0].ImportRow(ds.Tables[0].Rows[i]);
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
                            dsRep.Tables[0].ImportRow(ds.Tables[0].Rows[i]);
                        }
                    }
                }
            }

            return dsRep;
        }

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
                        sbRetMsg.Clear().Append(Msg.GetMsg(ct, "VA011_RequisitionNotCreated"));
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
                //vikas  SetSourcehouse
                int _CountDTD001 = Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(AD_MODULEINFO_ID) FROM AD_MODULEINFO WHERE PREFIX='DTD001_'"));
                if (_CountDTD001 > 0)
                {
                    requisitionReps.SetDTD001_MWarehouseSource_ID(Rep.M_WarehouseSource_ID);
                }
                requisitionReps.SetAD_Org_ID(wh.GetAD_Org_ID());
                requisitionReps.SetM_Warehouse_ID(wh.GetM_Warehouse_ID());

                if (!requisitionReps.Save())
                {
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

        public bool GetModuleInfo(string _prefix)
        {
            bool exist = false;

            if (Env.HasModulePrefix(_prefix, out aInfo))
            {
                exist = true;
            }
            return exist;

        }
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
        public int M_Product_ID { get; set; }
        public int M_Warehouse_ID { get; set; }
        public string RepType { get; set; }
        public decimal Max { get; set; }
        public decimal Min { get; set; }
        public decimal Qty { get; set; }
        public decimal OrderPack { get; set; }
        public int SourceWarehouse { get; set; }
    }
}