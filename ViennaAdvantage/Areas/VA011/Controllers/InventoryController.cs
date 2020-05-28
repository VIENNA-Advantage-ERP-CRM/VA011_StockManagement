using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VAdvantage.Utility;
using VA011.Models;

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

        public JsonResult GetProductDetails(int M_Product_ID)
        {
            Ctx ct = Session["ctx"] as Ctx;
            InventoryModel model = new InventoryModel();
            DataSet dsProd = model.GetProductDetails(M_Product_ID, ct);
            return Json(JsonConvert.SerializeObject(dsProd), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetProdCats(int pageNo, int pageSize)
        {
            Ctx ct = Session["ctx"] as Ctx;
            InventoryModel model = new InventoryModel();
            List<ProdCatInfo> prods = model.GetProdCats(pageNo, pageSize, ct);
            return Json(JsonConvert.SerializeObject(prods), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetOrganizations()
        {
            Ctx ct = Session["ctx"] as Ctx;
            InventoryModel model = new InventoryModel();
            List<NameIDClass> prods = model.GetOrganizations(ct);
            return Json(JsonConvert.SerializeObject(prods), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetOrgs(string value, bool fill)
        {
            Ctx ct = Session["ctx"] as Ctx;
            InventoryModel model = new InventoryModel();
            List<NameIDClass> prods = model.GetOrganizations(ct, value, fill);
            return Json(JsonConvert.SerializeObject(prods), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetWarehouse(string value, bool fill)
        {
            Ctx ct = Session["ctx"] as Ctx;
            InventoryModel model = new InventoryModel();
            List<NameIDClass> prods = model.GetWarehouse(ct, value, fill);
            return Json(JsonConvert.SerializeObject(prods), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetOrgWarehouse(string value, List<int> orgs, bool fill)
        {
            Ctx ct = Session["ctx"] as Ctx;
            InventoryModel model = new InventoryModel();
            List<NameIDClass> whs = model.GetOrgWarehouse(ct, value, orgs, fill);
            return Json(JsonConvert.SerializeObject(whs), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetOrgWarehouseAll(string value, List<int> orgs, bool fill)
        {
            Ctx ct = Session["ctx"] as Ctx;
            InventoryModel model = new InventoryModel();
            List<NameIDClass> whs = model.GetOrgWarehouseAll(ct, value, orgs, fill);
            return Json(JsonConvert.SerializeObject(whs), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPLV(string value, bool fill)
        {
            Ctx ct = Session["ctx"] as Ctx;
            InventoryModel model = new InventoryModel();
            List<NameIDClass> prods = model.GetPLV(ct, value, fill);
            return Json(JsonConvert.SerializeObject(prods), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSupplier(string value, bool fill)
        {
            Ctx ct = Session["ctx"] as Ctx;
            InventoryModel model = new InventoryModel();
            List<NameIDClass> prods = model.GetSupplier(ct, value, fill);
            return Json(JsonConvert.SerializeObject(prods), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetProductCategories(string value, bool fill)
        {
            Ctx ct = Session["ctx"] as Ctx;
            InventoryModel model = new InventoryModel();
            List<NameIDClass> prods = model.GetProductCategories(ct, value, fill);
            return Json(JsonConvert.SerializeObject(prods), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetProducts(string searchText, string warehouse_IDs, string org_IDs, string plv_IDs, string supp_IDs, string prodCat_IDs, int pageNo, int pageSize)
        {
            Ctx ct = Session["ctx"] as Ctx;
            InventoryModel model = new InventoryModel();
            List<Products> prods = model.GetProducts(searchText, warehouse_IDs, org_IDs, plv_IDs, supp_IDs, prodCat_IDs, pageNo, pageSize, ct);
            return Json(JsonConvert.SerializeObject(prods), JsonRequestBehavior.AllowGet);
        }


        public JsonResult GetProductsAll(int AD_Client_ID)
        {
            Ctx ct = Session["ctx"] as Ctx;
            InventoryModel model = new InventoryModel();
            List<NameIDClass> prods = model.GetProductsAll(ct);
            return Json(JsonConvert.SerializeObject(prods), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetProductCount(string searchText, string warehouse_IDs, string org_IDs, string plv_IDs, string supp_IDs, string prodCat_IDs)
        {
            Ctx ct = Session["ctx"] as Ctx;
            InventoryModel model = new InventoryModel();
            int prodCount = model.GeProductCount(searchText, warehouse_IDs, org_IDs, plv_IDs, supp_IDs, prodCat_IDs, ct);
            return Json(JsonConvert.SerializeObject(prodCount), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetReplenish(List<Int32> ColumnName, List<int> whIDs)
        {
            Ctx ct = Session["ctx"] as Ctx;
            InventoryModel model = new InventoryModel();
            List<Decimal?> qtyOrdered = model.GeReplenishment(ColumnName, whIDs, ct);
            return Json(JsonConvert.SerializeObject(qtyOrdered), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetReplenishments(List<int> Warehouses, int M_Product_ID)
        {
            Ctx ct = Session["ctx"] as Ctx;
            InventoryModel model = new InventoryModel();
            DataSet dsReps = model.GetReplenishments(Warehouses, M_Product_ID, ct);
            return Json(JsonConvert.SerializeObject(dsReps), JsonRequestBehavior.AllowGet);
        }

        public JsonResult SaveReplenishment(int M_Product_ID, int M_Warehouse_ID, string Type, Decimal Min, Decimal Max, Decimal Qty, Decimal OrderPack, int SourceWarehouse)
        {
            Ctx ct = Session["ctx"] as Ctx;
            InventoryModel model = new InventoryModel();
            String res = model.SaveReplenishment(M_Product_ID, M_Warehouse_ID, Type, Min, Max, Qty, OrderPack, SourceWarehouse, ct);
            return Json(JsonConvert.SerializeObject(res), JsonRequestBehavior.AllowGet);
        }

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

        public JsonResult GenerateReplenishmentReport(int M_Warehouse_ID, int C_BPartner_ID, int C_DocType_ID, string DocStatus, string Create, string OrderPack)
        {
            Ctx ct = Session["ctx"] as Ctx;
            InventoryModel model = new InventoryModel();
            DataSet ds = model.GenerateReplenishmentReport(M_Warehouse_ID, C_BPartner_ID, C_DocType_ID, DocStatus, Create, OrderPack, ct);
            return Json(JsonConvert.SerializeObject(ds), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GenerateReps(List<RepCreateData> Reps)
        {
            Ctx ct = Session["ctx"] as Ctx;
            InventoryModel model = new InventoryModel();
            //DataSet ds = model.GenerateReplenishmentReport(M_Warehouse_ID, C_BPartner_ID, C_DocType_ID, DocStatus, Create, OrderPack, ct);
            string retMsg = model.GenerateReps(Reps, ct);
            return Json(JsonConvert.SerializeObject(retMsg), JsonRequestBehavior.AllowGet);
        }

        public JsonResult SaveReplenishmentRuleAll(List<RepRule> RepAll)
        {
            Ctx ct = Session["ctx"] as Ctx;
            InventoryModel model = new InventoryModel();
            //DataSet ds = model.GenerateReplenishmentReport(M_Warehouse_ID, C_BPartner_ID, C_DocType_ID, DocStatus, Create, OrderPack, ct);
            string retMsg = model.SaveRepRuleAll(RepAll, ct);
            return Json(JsonConvert.SerializeObject(retMsg), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetModuleInfo(string _prefix)
        {
            bool exist = false;
            InventoryModel model = new InventoryModel();
            exist = model.GetModuleInfo(_prefix);
            return Json(JsonConvert.SerializeObject(exist), JsonRequestBehavior.AllowGet);
        }
    }
}