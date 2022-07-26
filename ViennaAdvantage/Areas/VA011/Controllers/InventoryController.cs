using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VAdvantage.Utility;
using VA011.Models;
using VAdvantage.Model;

namespace VA011.Controllers
{
    public class InventoryController : Controller
    {
        //
        // GET: /VA011/Inventory/

        public ActionResult Index(int windowno)
        {
            //return PartialView();
            return View();
        }
        /// <summary>
        /// GetProductDetails Method
        /// </summary>
        /// <param name="M_Product_ID">M_Product_ID</param>
        /// <returns>Product</returns>
        public JsonResult GetProductDetails(int M_Product_ID)
        {
            Ctx ct = Session["ctx"] as Ctx;
            InventoryModel model = new InventoryModel();
            DataSet dsProd = model.GetProductDetails(M_Product_ID, ct);
            return Json(JsonConvert.SerializeObject(dsProd), JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// GetProdCats Method
        /// </summary>
        /// <param name="pageNo">pageNo</param>
        /// <param name="pageSize">pageSize</param>
        /// <returns>ProdCats</returns>
        public JsonResult GetProdCats(int pageNo, int pageSize)
        {
            Ctx ct = Session["ctx"] as Ctx;
            InventoryModel model = new InventoryModel();
            List<ProdCatInfo> prods = model.GetProdCats(pageNo, pageSize, ct);
            return Json(JsonConvert.SerializeObject(prods), JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// GetOrganizations Method
        /// </summary>
        /// <returns>Organizations</returns>
        public JsonResult GetOrganizations()
        {
            Ctx ct = Session["ctx"] as Ctx;
            InventoryModel model = new InventoryModel();
            List<NameIDClass> prods = model.GetOrganizations(ct);
            return Json(JsonConvert.SerializeObject(prods), JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// GetOrgs Method
        /// </summary>
        /// <param name="value">value</param>
        /// <param name="fill">fill</param>
        /// <returns></returns>
        public JsonResult GetOrgs(string value, bool fill)
        {
            Ctx ct = Session["ctx"] as Ctx;
            InventoryModel model = new InventoryModel();
            List<NameIDClass> prods = model.GetOrganizations(ct, value, fill);
            return Json(JsonConvert.SerializeObject(prods), JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// selectCartGrid Method
        /// </summary>
        /// <param name="M_Product_ID">M_Product_ID</param>
        /// <returns>pop window</returns>
        public JsonResult selectCartGrid(int M_Product_ID)
        {
            Ctx ct = Session["ctx"] as Ctx;
            InventoryModel model = new InventoryModel();
            int prodCount = model.selectCartGrid( ct, M_Product_ID);
            return Json(JsonConvert.SerializeObject(prodCount), JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// GetWarehouse Method
        /// </summary>
        /// <param name="value">value</param>
        /// <param name="fill">fill</param>
        /// <returns>Warehouse</returns>
        public JsonResult GetWarehouse(string value, bool fill)
        {
            Ctx ct = Session["ctx"] as Ctx;
            InventoryModel model = new InventoryModel();
            List<NameIDClass> prods = model.GetWarehouse(ct, value, fill);
            return Json(JsonConvert.SerializeObject(prods), JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// GetOrgWarehouse Method
        /// </summary>
        /// <param name="value">value</param>
        /// <param name="orgs">orgs</param>
        /// <param name="fill">fill</param>
        /// <returns>OrgWarehouse</returns>
        public JsonResult GetOrgWarehouse(string value, List<int> orgs, bool fill)
        {
            Ctx ct = Session["ctx"] as Ctx;
            InventoryModel model = new InventoryModel();
            List<NameIDClass> whs = model.GetOrgWarehouse(ct, value, orgs, fill);
            return Json(JsonConvert.SerializeObject(whs), JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// GetOrgWarehouseAll Method
        /// </summary>
        /// <param name="value">value</param>
        /// <param name="orgs">orgs</param>
        /// <param name="fill">fill</param>
        /// <returns>All OrgWarehouse</returns>
        public JsonResult GetOrgWarehouseAll(string value, List<int> orgs, bool fill)
        {
            Ctx ct = Session["ctx"] as Ctx;
            InventoryModel model = new InventoryModel();
            List<NameIDClass> whs = model.GetOrgWarehouseAll(ct, value, orgs, fill);
            return Json(JsonConvert.SerializeObject(whs), JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// GetPLV Method
        /// </summary>
        /// <param name="value">value</param>
        /// <param name="fill">fill</param>
        /// <returns>PLV</returns>
        public JsonResult GetPLV(string value, bool fill)
        {
            Ctx ct = Session["ctx"] as Ctx;
            InventoryModel model = new InventoryModel();
            List<NameIDClass> prods = model.GetPLV(ct, value, fill);
            return Json(JsonConvert.SerializeObject(prods), JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// GetProductCategories Method
        /// </summary>
        /// <param name="value">value</param>
        /// <param name="fill">fill</param>
        /// <returns>ProductCategories</returns>
        public JsonResult GetProductCategories(string value, bool fill)
        {
            Ctx ct = Session["ctx"] as Ctx;
            InventoryModel model = new InventoryModel();
            List<NameIDClass> prods = model.GetProductCategories(ct, value, fill);
            return Json(JsonConvert.SerializeObject(prods), JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// GetProducts Method
        /// </summary>
        /// <param name="searchText">searchText</param>
        /// <param name="warehouse_IDs">ID</param>
        /// <param name="org_IDs">ID</param>
        /// <param name="plv_IDs">ID</param>
        /// <param name="supp_IDs">ID</param>
        /// <param name="prodCat_IDs">ID</param>
        /// <param name="pageNo">pageNo</param>
        /// <param name="pageSize">pageSize</param>
        /// <returns></returns>
        public JsonResult GetProducts(string searchText, string warehouse_IDs, string org_IDs, string plv_IDs, string supp_IDs, string prodCat_IDs, int pageNo, int pageSize)
        {
            Ctx ct = Session["ctx"] as Ctx;
            InventoryModel model = new InventoryModel();
            List<Products> prods = model.GetProducts(searchText, warehouse_IDs, org_IDs, plv_IDs, supp_IDs, prodCat_IDs, pageNo, pageSize, ct);
            return Json(JsonConvert.SerializeObject(prods), JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// GetProductsAll Method
        /// </summary>
        /// <param name="AD_Client_ID">ID</param>
        /// <returns>All Products</returns>
        public JsonResult GetProductsAll(int AD_Client_ID)
        {
            Ctx ct = Session["ctx"] as Ctx;
            InventoryModel model = new InventoryModel();
            List<NameIDClass> prods = model.GetProductsAll(ct);
            return Json(JsonConvert.SerializeObject(prods), JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// GetProductCount Method
        /// </summary>
        /// <param name="searchText">searchText</param>
        /// <param name="warehouse_IDs">ID</param>
        /// <param name="org_IDs">ID</param>
        /// <param name="plv_IDs">ID</param>
        /// <param name="supp_IDs">ID</param>
        /// <param name="prodCat_IDs">ID</param>
        /// <returns></returns>
        public JsonResult GetProductCount(string searchText, string warehouse_IDs, string org_IDs, string plv_IDs, string supp_IDs, string prodCat_IDs)
        {
            Ctx ct = Session["ctx"] as Ctx;
            InventoryModel model = new InventoryModel();
            int prodCount = model.GeProductCount(searchText, warehouse_IDs, org_IDs, plv_IDs, supp_IDs, prodCat_IDs, ct);
            return Json(JsonConvert.SerializeObject(prodCount), JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// GetReplenish Method
        /// </summary>
        /// <param name="ColumnName">Name</param>
        /// <param name="whIDs">ID</param>
        /// <returns>Replenish</returns>
        public JsonResult GetReplenish(List<Int32> ColumnName, List<int> whIDs)
        {
            Ctx ct = Session["ctx"] as Ctx;
            InventoryModel model = new InventoryModel();
            List<Decimal?> qtyOrdered = model.GeReplenishment(ColumnName, whIDs, ct);
            return Json(JsonConvert.SerializeObject(qtyOrdered), JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// GetReplenishments Method
        /// </summary>
        /// <param name="Warehouses">Warehouses</param>
        /// <param name="M_Product_ID">ID</param>
        /// <returns>Replenishments</returns>
        public JsonResult GetReplenishments(List<int> Warehouses, int M_Product_ID)
        {
            Ctx ct = Session["ctx"] as Ctx;
            InventoryModel model = new InventoryModel();
            List<RepGet> repGets = model.GetReplenishments(Warehouses, M_Product_ID, ct);
            //DataSet dsReps = model.GetReplenishments(Warehouses, M_Product_ID, ct);
            return Json(JsonConvert.SerializeObject(repGets), JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// SaveReplenishment Method
        /// </summary>
        /// <param name="M_Product_ID">M_Product_ID</param>
        /// <param name="M_Warehouse_ID">ID</param>
        /// <param name="Type">Type</param>
        /// <param name="Min">Min</param>
        /// <param name="Max">Max</param>
        /// <param name="Qty">Qty</param>
        /// <param name="OrderPack">OrderPack</param>
        /// <param name="SourceWarehouse">SourceWarehouse</param>
        /// <returns>Save Replenishment</returns>
        public JsonResult SaveReplenishment(int M_Product_ID, int M_Warehouse_ID, string Type, Decimal Min, Decimal Max, Decimal Qty, Decimal OrderPack, int SourceWarehouse)
        {
            Ctx ct = Session["ctx"] as Ctx;
            InventoryModel model = new InventoryModel();
            String res = model.SaveReplenishment(M_Product_ID, M_Warehouse_ID, Type, Min, Max, Qty, OrderPack, SourceWarehouse, ct);
            return Json(JsonConvert.SerializeObject(res), JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// saveInventoryCount method
        /// </summary>
        /// <param name="ColumnName">name</param>
        /// <returns>Count</returns>
        [HttpPost]
        public JsonResult saveInventoryCount(string ColumnName)
        {
            string value = "";
            if (Session["ctx"] != null)
            {
                VAdvantage.Utility.Ctx ctx = Session["ctx"] as Ctx;
                InventoryModel model = new InventoryModel();
                value = model.saveInventoryCount(ColumnName, ctx);
            }
            return Json(new { result = value }, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// SaveInventory method
        /// </summary>
        /// <param name="Count_ID">ID</param>
        /// <param name="ColumnName">Name</param>
        /// <returns>Inventory</returns>
        [HttpPost]
        public JsonResult SaveInventory(int Count_ID, List<PriceInfo> ColumnName)
        {
            string value = "";
            if (Session["ctx"] != null)
            {
                VAdvantage.Utility.Ctx ctx = Session["ctx"] as Ctx;
                InventoryModel model = new InventoryModel();
                value = model.saveInventory(Count_ID, ColumnName, ctx);
            }
            return Json(new { result = value }, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// updateInventory method
        /// </summary>
        /// <param name="ColumnName">Name</param>
        /// <returns>Inventory</returns>
        [HttpPost]
        public JsonResult updateInventory(List<PriceInfo> ColumnName)
        {
            string value = "";
            if (Session["ctx"] != null)
            {
                VAdvantage.Utility.Ctx ctx = Session["ctx"] as Ctx;
                InventoryModel model = new InventoryModel();
                value = model.updateInventory(ColumnName, ctx);
            }
            return Json(new { result = value }, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// deleteInventory Methpd
        /// </summary>
        /// <param name="ColumnName">Name</param>
        /// <returns>delete Inventory</returns>
        [HttpPost]
        public JsonResult deleteInventory(List<PriceInfo> ColumnName)
        {
            string value = "";
            if (Session["ctx"] != null)
            {
                VAdvantage.Utility.Ctx ctx = Session["ctx"] as Ctx;
                InventoryModel model = new InventoryModel();
                value = model.deleteInventory(ColumnName, ctx);
            }
            return Json(new { result = value }, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// GenerateReplenishmentReport Method
        /// </summary>
        /// <param name="M_Warehouse_ID">ID</param>
        /// <param name="C_BPartner_ID">ID</param>
        /// <param name="C_DocType_ID">ID</param>
        /// <param name="DocStatus">Status</param>
        /// <param name="Create">Create</param>
        /// <param name="OrderPack">OrderPack</param>
        /// <returns>Replenishment Report</returns>
        public JsonResult GenerateReplenishmentReport(int M_Warehouse_ID, int C_BPartner_ID, int C_DocType_ID, string DocStatus, string Create, string OrderPack)
        {
            Ctx ct = Session["ctx"] as Ctx;
            InventoryModel model = new InventoryModel();
            DataSet ds = model.GenerateReplenishmentReport(M_Warehouse_ID, C_BPartner_ID, C_DocType_ID, DocStatus, Create, OrderPack, ct);
            return Json(JsonConvert.SerializeObject(ds), JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// GenerateReps Method
        /// </summary>
        /// <param name="Reps">Reps</param>
        /// <returns>Reps</returns>
        public JsonResult GenerateReps(List<RepCreateData> Reps)
        {
            Ctx ct = Session["ctx"] as Ctx;
            InventoryModel model = new InventoryModel();
            //DataSet ds = model.GenerateReplenishmentReport(M_Warehouse_ID, C_BPartner_ID, C_DocType_ID, DocStatus, Create, OrderPack, ct);
            string retMsg = model.GenerateReps(Reps, ct);
            return Json(JsonConvert.SerializeObject(retMsg), JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// SaveReplenishmentRuleAll Method
        /// </summary>
        /// <param name="RepAll">RepAll</param>
        /// <returns>Replenishment Rule All</returns>
        public JsonResult SaveReplenishmentRuleAll(List<RepRule> RepAll)
        {
            Ctx ct = Session["ctx"] as Ctx;
            InventoryModel model = new InventoryModel();
            //DataSet ds = model.GenerateReplenishmentReport(M_Warehouse_ID, C_BPartner_ID, C_DocType_ID, DocStatus, Create, OrderPack, ct);
            string retMsg = model.SaveRepRuleAll(RepAll, ct);
            return Json(JsonConvert.SerializeObject(retMsg), JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// GetModuleInfo Method
        /// </summary>
        /// <param name="_prefix">_prefix</param>
        /// <returns>Module Info</returns>
        public JsonResult GetModuleInfo(string _prefix)
        {
            bool exist = false;
            InventoryModel model = new InventoryModel();
            exist = model.GetModuleInfo(_prefix);
            return Json(JsonConvert.SerializeObject(exist), JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// GetSupplier Method
        /// </summary>
        /// <returns>Vender</returns>
        public JsonResult GetSupplier()
        {
            Ctx ct = Session["ctx"] as Ctx;
            InventoryModel model = new InventoryModel();
            List<NameIDClass> prods = model.GetSupplier(ct);
            return Json(JsonConvert.SerializeObject(prods), JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// GetCart Method
        /// </summary>
        /// <returns>List of Product</returns>
        public JsonResult GetCart()
        {
            Ctx ct = Session["ctx"] as Ctx;
            InventoryModel model = new InventoryModel();
            List<NameIDClass> prods = model.GetCart(ct);
            return Json(JsonConvert.SerializeObject(prods), JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// GetDocType Method
        /// </summary>
        /// <returns>List of Document</returns>
        public JsonResult GetDocType()
        {
            Ctx ct = Session["ctx"] as Ctx;
            InventoryModel model = new InventoryModel();
            List<DocType> docTypes = model.GetDocType(ct);
            return Json(JsonConvert.SerializeObject(docTypes), JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Doc Status
        /// </summary>
        /// <returns>Status</returns>
        public JsonResult GetDocStatus()
        {
            Ctx ct = Session["ctx"] as Ctx;
            InventoryModel model = new InventoryModel();
            List<ValueNamePair> prods = model.GetDocStatus(ct);
            return Json(JsonConvert.SerializeObject(prods), JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Get Replenish Type Method
        /// </summary>
        /// <returns>Replenish Type</returns>
        public JsonResult GetReplenishType()
        {
            Ctx ct = Session["ctx"] as Ctx;
            InventoryModel model = new InventoryModel();
            List<ValueNamePair> prods = model.GetReplenishType(ct);
            return Json(JsonConvert.SerializeObject(prods), JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Create Combo Method
        /// </summary>
        /// <returns>Combo</returns>
        public JsonResult CreateCombo()
        {
            Ctx ct = Session["ctx"] as Ctx;
            InventoryModel model = new InventoryModel();
            List<ValueNamePair> prods = model.CreateCombo(ct);
            return Json(JsonConvert.SerializeObject(prods), JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Get UOM Method
        /// </summary>
        /// <returns></returns>
        public JsonResult GetUOM()
        {
            Ctx ct = Session["ctx"] as Ctx;
            InventoryModel model = new InventoryModel();
            List<NameIDClass> prods = model.GetUOM(ct);
            return Json(JsonConvert.SerializeObject(prods), JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Load cart Method
        /// </summary>
        /// <param name="VAICNT_InventoryCount_ID">VAICNT_InventoryCount_ID</param>
        /// <returns>data</returns>
        public JsonResult LoadBindcart(int VAICNT_InventoryCount_ID)
        {
            Ctx ct = Session["ctx"] as Ctx;
            InventoryModel model = new InventoryModel();
            List<PriceInfo> prods = model.LoadBindcart(ct, VAICNT_InventoryCount_ID);
            return Json(JsonConvert.SerializeObject(prods), JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Substitute Grid Method
        /// </summary>
        /// <param name="M_Product_ID">M_Product_ID</param>
        /// <returns>data</returns>

        public JsonResult LoadSubstituteGrid(int M_Product_ID, List<int> selWH)
        {
            Ctx ct = Session["ctx"] as Ctx;
            InventoryModel model = new InventoryModel();
            List<Substitute> prods = model.LoadSubstituteGrid(ct, M_Product_ID, selWH);
            return Json(JsonConvert.SerializeObject(prods), JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// LoadSuppliersRightGrid Method
        /// </summary>
        /// <param name="M_Product_ID">M_Product_ID</param>
        /// <returns>Supplier</returns>
        public JsonResult LoadSuppliersRightGrid(int M_Product_ID)
        {
            Ctx ct = Session["ctx"] as Ctx;
            InventoryModel model = new InventoryModel();
            List<PriceInfo> prods = model.LoadSuppliersRightGrid(ct, M_Product_ID);
            return Json(JsonConvert.SerializeObject(prods), JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Kits grid Methd
        /// </summary>
        /// <param name="M_ProductBOM_ID">M_ProductBOM_ID</param>
        /// <returns>value</returns>
        public JsonResult LoadKitsGrid(int M_ProductBOM_ID, List<int> selWH)
        {
            Ctx ct = Session["ctx"] as Ctx;
            InventoryModel model = new InventoryModel();
            List<Kits> prods = model.LoadKitsGrid(ct, M_ProductBOM_ID, selWH);
            return Json(JsonConvert.SerializeObject(prods), JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// LoadVariantGrid Method
        /// </summary>
        /// <param name="M_Product_ID">M_Product_ID</param>
        /// <param name="selWH">selWH</param>
        /// <param name="orgs">orgs</param>
        /// <returns>Selected value</returns>
        public JsonResult LoadVariantGrid(int M_Product_ID, List<int> selWH, string orgs)
        {
            Ctx ct = Session["ctx"] as Ctx;
            InventoryModel model = new InventoryModel();
            List<Variant> prods = model.LoadVariantGrid(ct, M_Product_ID, selWH, orgs);
            return Json(JsonConvert.SerializeObject(prods), JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// LocatorGrid Method
        /// </summary>
        /// <param name="M_Product_ID">M_Product_ID</param>
        /// <returns>Locator</returns>
        public JsonResult LocatorGrid(int M_Product_ID,List<int> Warehouses,string orgS)
        {
            Ctx ct = Session["ctx"] as Ctx;
            InventoryModel model = new InventoryModel();
            List<Transaction> prods = model.LocatorGrid(ct, M_Product_ID, Warehouses, orgS);
            return Json(JsonConvert.SerializeObject(prods), JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// LoadDemandGrid Methdo
        /// </summary>
        /// <param name="M_Product_ID">M_Product_ID</param>
        /// <param name="selWH">selWH</param>
        /// <param name="orgs">orgs</param>
        /// <returns></returns>
        public JsonResult LoadDemandGrid(int M_Product_ID, List<int> selWH, string orgs)
        {
            Ctx ct = Session["ctx"] as Ctx;
            InventoryModel model = new InventoryModel();
            List<Transaction> prods = model.LoadDemandGrid(ct, M_Product_ID, selWH, orgs);
            return Json(JsonConvert.SerializeObject(prods), JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// LoadOrderedGrid Method
        /// </summary>
        /// <param name="M_Product_ID">M_Product_ID</param>
        /// <returns>Order value</returns>
        public JsonResult LoadOrderedGrid(int M_Product_ID,List<int> Warehouses)
        {
            Ctx ct = Session["ctx"] as Ctx;
            InventoryModel model = new InventoryModel();
            List<Order> prods = model.LoadOrderedGrid(ct, M_Product_ID, Warehouses);
            return Json(JsonConvert.SerializeObject(prods), JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// LoadTransactionsGrid Method
        /// </summary>
        /// <param name="M_Product_ID">M_Product_ID</param>
        /// <returns>Trnsactions value</returns>
        public JsonResult LoadTransactionsGrid(int M_Product_ID, List<int> selWH)
        {
            Ctx ct = Session["ctx"] as Ctx;
            InventoryModel model = new InventoryModel();
            List<Transaction> prods = model.LoadTransactionsGrid(ct, M_Product_ID, selWH);
            return Json(JsonConvert.SerializeObject(prods), JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// LoadReplenishmentPopGrid Method
        /// </summary>
        /// <param name="M_Product_ID">M_Product_ID</param>
        /// <param name="M_Warehouse_ID">M_Warehouse_ID</param>
        /// <param name="selWH">selWH</param>
        /// <returns>Replenishment</returns>
        public JsonResult LoadReplenishmentPopGrid(int M_Warehouse_ID,string sqlWhere)
        {
            Ctx ct = Session["ctx"] as Ctx;
            InventoryModel model = new InventoryModel();
            List<RepRule> prods = model.LoadReplenishmentPopGrid(ct, M_Warehouse_ID, sqlWhere);
            return Json(JsonConvert.SerializeObject(prods), JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// LoadReplenishmentBGrid Methdo
        /// </summary>
        /// <param name="M_Product_ID">M_Product_ID</param>
        /// <param name="selWH">selWH</param>
        /// <returns>Replenishment</returns>
        public JsonResult LoadReplenishmentBGrid(int M_Product_ID, List<int> selWH)
        {
            Ctx ct = Session["ctx"] as Ctx;
            InventoryModel model = new InventoryModel();
            List<RepRule> prods = model.LoadReplenishmentBGrid(ct, M_Product_ID, selWH);
            return Json(JsonConvert.SerializeObject(prods), JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Zoom Window method
        /// </summary>
        /// <param name="windowName">windowName</param>
        /// <returns></returns>
        public JsonResult LoadWindow(string windowName)
        {
            InventoryModel model = new InventoryModel();
            return Json(JsonConvert.SerializeObject(model.LoadWindow(windowName)), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Load Related Grid
        /// </summary>
        /// <param name="M_Product_ID">Product ID</param>
        /// <param name="selWH">Selected Warehouse</param>
        /// <returns>Result</returns>
        public JsonResult LoadRelatedGrid(int M_Product_ID, List<int> selWH)
        {
            Ctx ct = Session["ctx"] as Ctx;
            InventoryModel model = new InventoryModel();
            List<Substitute> prods = model.LoadRelatedGrid(ct, M_Product_ID, selWH);
            return Json(JsonConvert.SerializeObject(prods), JsonRequestBehavior.AllowGet);
        }
    }
}