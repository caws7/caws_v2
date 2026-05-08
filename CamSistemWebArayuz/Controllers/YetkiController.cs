using CamSistemDataLayer.Helpers;
using CamSistemDataLayer.Models;
using CamSistemDataLayer.Repos;
using CamSistemWebArayuz.Attributes;
using CamSistemWebArayuz.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace CamSistemWebArayuz.Controllers
{
    [SessionController]    
    [AuthLog(Roles = "YETKİ")]
    public class YetkiController : Controller
    {
        KullaniciRepo kullaniciRepo;
        KullaniciRolRepo kullaniciRolRepo;
        RolRepo rolRepo;
        RolSayfaYetkiRepo rolSayfaYetkiRepo;

        // GET: Yetki
        [AuthLog(Roles = "YETKİ,GORUNTULEME")]
        public ActionResult Index()
        {
            TempData["loader"] = "Lütfen bekleyiniz...";
            TempData["ActiveMenu"] = "YetkiSayfasi";

            kullaniciRolRepo = new KullaniciRolRepo();
            rolRepo = new RolRepo();

            KullaniciRolModel multiModel = new KullaniciRolModel();
            multiModel.KullaniciRols = kullaniciRolRepo.GetAll();
            multiModel.Rols = rolRepo.GetAll();
            return View(multiModel);
        }

        [HttpGet]
        [AuthLog(Roles = "YENIKAYIT")]
        public ActionResult RolTanimlama()
        {
            TempData["loader"] = "Lütfen bekleyiniz...";
            ViewData["error"] = null;
            List<TreeViewNode> nodes = new List<TreeViewNode>();
            SayfaRepo sayfaRepo = new SayfaRepo();
            YetkiRepo yetkiRepo = new YetkiRepo();

            foreach (Sayfa sayfa in sayfaRepo.GetAll().ToList())
            {
                nodes.Add(new TreeViewNode { id = sayfa.Id.ToString(), parent = "#", text = TurkishTextNormalizer.NormalizeDisplayText(sayfa.SayfaAdi) });

                //Loop and add the Child Nodes.
                foreach (Yetki yetki in yetkiRepo.GetAll().ToList())
                {
                    nodes.Add(new TreeViewNode { id = yetki.Id.ToString() + "i" + sayfa.Id.ToString() + "-" + yetki.Id.ToString() + "i" + sayfa.Id.ToString(), parent = sayfa.Id.ToString(), text = TurkishTextNormalizer.NormalizeDisplayText(yetki.YetkiAdi) });
                }
            }

            //Serialize to JSON string.
            ViewBag.SayfaYetkiTree = (new JavaScriptSerializer()).Serialize(nodes);

            return View();
        }

        [HttpPost]
        [AuthLog(Roles = "YENIKAYIT")]
        public ActionResult RolTanimlama(string RolAdi, string selectedItems)
        {
            rolRepo = new RolRepo();
            Rol rol = rolRepo.FindBy(e => e.RolAdi.Equals(RolAdi)).FirstOrDefault();

            if (rol != null)
            {
                List<TreeViewNode> nodes = new List<TreeViewNode>();
                SayfaRepo sayfaRepo = new SayfaRepo();
                YetkiRepo yetkiRepo = new YetkiRepo();

                foreach (Sayfa sayfa in sayfaRepo.GetAll().ToList())
                {
                    nodes.Add(new TreeViewNode { id = sayfa.Id.ToString(), parent = "#", text = TurkishTextNormalizer.NormalizeDisplayText(sayfa.SayfaAdi) });

                    //Loop and add the Child Nodes.
                    foreach (Yetki yetki in yetkiRepo.GetAll().ToList())
                    {
                        nodes.Add(new TreeViewNode { id = yetki.Id.ToString() + "i" + sayfa.Id.ToString() + "-" + yetki.Id.ToString() + "i" + sayfa.Id.ToString(), parent = sayfa.Id.ToString(), text = TurkishTextNormalizer.NormalizeDisplayText(yetki.YetkiAdi) });
                    }
                }

                ViewBag.SayfaYetkiTree = (new JavaScriptSerializer()).Serialize(nodes);
                ViewData["error"] = "someErrorMessage";
                return View();
            }

            rol = new Rol();
            rol.RolAdi = RolAdi.ToUpper();
            rol = rolRepo.SaveAndReturnEntity(rol);
            List<TreeViewNode> items = new List<TreeViewNode>();

            if (!string.IsNullOrWhiteSpace(selectedItems))
            {
                try
                {
                    items = (new JavaScriptSerializer()).Deserialize<List<TreeViewNode>>(selectedItems) ?? new List<TreeViewNode>();
                }
                catch
                {
                    items = new List<TreeViewNode>();
                }
            }
            foreach (var item in items)
            {
                if (item.id.Contains("i"))
                {
                    rolSayfaYetkiRepo = new RolSayfaYetkiRepo();
                    RolSayfaYetki rolSayfaYetki = new RolSayfaYetki();
                    string[] ids = item.id.Split('i');

                    rolSayfaYetki.RolId = rol.Id;
                    rolSayfaYetki.SayfaId = Convert.ToInt32(ids[1]);
                    rolSayfaYetki.YetkiId = Convert.ToInt32(ids[0]);
                    rolSayfaYetkiRepo.AddAndSave(rolSayfaYetki);
                }
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        [AuthLog(Roles = "DUZENLEME")]
        public ActionResult RolDuzenleme(int RolId)
        {
            List<TreeViewNode> nodes = new List<TreeViewNode>();
            SayfaRepo sayfaRepo = new SayfaRepo();
            YetkiRepo yetkiRepo = new YetkiRepo();
            rolRepo = new RolRepo();

            foreach (Sayfa sayfa in sayfaRepo.GetAll().ToList())
            {
                nodes.Add(new TreeViewNode { id = sayfa.Id.ToString(), parent = "#", text = TurkishTextNormalizer.NormalizeDisplayText(sayfa.SayfaAdi) });
                
                foreach (Yetki yetki in yetkiRepo.GetAll().ToList())
                {
                    nodes.Add(new TreeViewNode { id = yetki.Id.ToString() + "i" + sayfa.Id.ToString() + "-" + yetki.Id.ToString() + "i" + sayfa.Id.ToString(), parent = sayfa.Id.ToString(), text = TurkishTextNormalizer.NormalizeDisplayText(yetki.YetkiAdi) });
                }
            }

            List<TreeViewNode> selectedNodes = new List<TreeViewNode>();
            rolSayfaYetkiRepo = new RolSayfaYetkiRepo();
            List<RolSayfaYetki> rolSayfaYetkis = rolSayfaYetkiRepo.FindBy(e => e.RolId == RolId).ToList();

            List<int> sayfaIds = rolSayfaYetkiRepo.FindBy(e => e.RolId == RolId).Distinct().Select(e=>(int)e.SayfaId).ToList();
            List<Sayfa> sayfas = sayfaRepo.FindBy(e => sayfaIds.Contains(e.Id)).ToList();
            foreach (Sayfa sayfa in sayfas)
            {
                List<int> yetkiIds = rolSayfaYetkiRepo.FindBy(e => e.RolId == RolId && e.SayfaId == sayfa.Id).Distinct().Select(e=> (int)e.YetkiId).ToList();
                List<Yetki> yetkis = yetkiRepo.FindBy(e => yetkiIds.Contains(e.Id)).ToList();

                if(yetkis.Count() == 5)
                    selectedNodes.Add(new TreeViewNode { id = sayfa.Id.ToString(), parent = "#", text = TurkishTextNormalizer.NormalizeDisplayText(sayfa.SayfaAdi) });
                
                foreach (Yetki yetki in yetkis)
                {
                    selectedNodes.Add(new TreeViewNode { id = yetki.Id.ToString() + "i" + sayfa.Id.ToString() + "-" + yetki.Id.ToString() + "i" + sayfa.Id.ToString(), parent = sayfa.Id.ToString(), text = TurkishTextNormalizer.NormalizeDisplayText(yetki.YetkiAdi) });
                }
            }

            ViewBag.SeciliKayitSayisi = selectedNodes.Count();
            ViewBag.SeciliKayitlar = (new JavaScriptSerializer()).Serialize(selectedNodes);
            ViewBag.SayfaYetkiTree = (new JavaScriptSerializer()).Serialize(nodes);
            ViewBag.SelectedRolAdi = rolRepo.FindBy(e => e.Id == RolId).FirstOrDefault().RolAdi;

            return View();
        }

        [HttpPost]
        [AuthLog(Roles = "DUZENLEME")]
        public ActionResult RolDuzenleme(int RolId, string RolAdi, string selectedItems)
        {
            rolRepo = new RolRepo();
            Rol rol = rolRepo.FindBy(e => e.Id == RolId).FirstOrDefault();
            if (!rol.RolAdi.Equals(RolAdi))
            {
                rol.RolAdi = RolAdi;
                rolRepo.EditAndSave(rol);
            }

            //role ait tüm listeyi sil sonra yeni ekle
            RolSayfaYetkiRepo deleteRolSayfaYetkiRepo = new RolSayfaYetkiRepo();
            List<RolSayfaYetki> tumSayfaYetkileri = deleteRolSayfaYetkiRepo.FindBy(e => e.RolId == rol.Id).ToList();
            foreach (var item in tumSayfaYetkileri)
            {
                deleteRolSayfaYetkiRepo.DeleteAndSave(item);
            }

            List<TreeViewNode> items = (new JavaScriptSerializer()).Deserialize<List<TreeViewNode>>(selectedItems);
            foreach (var item in items)
            {
                if (item.id.Contains("i"))
                {
                    rolSayfaYetkiRepo = new RolSayfaYetkiRepo();
                    RolSayfaYetki rolSayfaYetki = new RolSayfaYetki();
                    string[] ids = item.id.Split('i');

                    rolSayfaYetki.RolId = rol.Id;
                    rolSayfaYetki.SayfaId = Convert.ToInt32(ids[1]);
                    rolSayfaYetki.YetkiId = Convert.ToInt32(ids[0]);
                    rolSayfaYetkiRepo.AddAndSave(rolSayfaYetki);
                }
            }

            return RedirectToAction("Index");            
        }

        [HttpPost]
        [AuthLog(Roles = "SILME")]
        public ActionResult KullaniciRolSil(int KullaniciRolId, string Sayfa)
        {
            TempData["loader"] = "Lütfen bekleyiniz...";

            try
            {
                kullaniciRolRepo = new KullaniciRolRepo();    
                if (Sayfa.Equals("kullaniciRol"))
                {
                    kullaniciRolRepo.DeleteAndSave(kullaniciRolRepo.FindBy(e => e.Id == KullaniciRolId).FirstOrDefault());

                    return Json("OK", JsonRequestBehavior.AllowGet);
                }
                else if (Sayfa.Equals("rol"))
                {
                    bool kayitVarMi = kullaniciRolRepo.FindBy(e => e.RolId == KullaniciRolId).ToList().Count > 0 ? true : false;
                    if (kayitVarMi)
                        return Json("NOK", JsonRequestBehavior.AllowGet);

                    rolRepo = new RolRepo();
                    rolSayfaYetkiRepo = new RolSayfaYetkiRepo();

                    List<RolSayfaYetki> rolSayfaYetkis = rolSayfaYetkiRepo.FindBy(e => e.RolId == KullaniciRolId).ToList();
                    foreach (var item in rolSayfaYetkis)
                    {
                        rolSayfaYetkiRepo.DeleteAndSave(item);
                    }

                    rolRepo.DeleteAndSave(rolRepo.FindBy(e => e.Id == KullaniciRolId).FirstOrDefault());

                    return Json("OK", JsonRequestBehavior.AllowGet);
                }

                return null;
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        [AuthLog(Roles = "YENIKAYIT")]
        public ActionResult KullaniciYetkilendirme()
        {
            TempData["loader"] = "Lütfen bekleyiniz...";
            kullaniciRepo = new KullaniciRepo();
            rolRepo = new RolRepo();
            ViewBag.Kullanicilar = kullaniciRepo.GetAll();
            ViewBag.Roller = rolRepo.GetAll()
                .AsEnumerable()
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = TurkishTextNormalizer.NormalizeDisplayText(x.RolAdi)
                })
                .ToList();
            return PartialView("KullaniciYetkilendirme");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthLog(Roles = "YENIKAYIT")]
        public ActionResult KullaniciYetkilendirme(KullaniciRol kullaniciRol)
        {
            if (ModelState.IsValid)
            {
                kullaniciRolRepo = new KullaniciRolRepo();
                kullaniciRepo = new KullaniciRepo();
                rolRepo = new RolRepo();
                ViewBag.Kullanicilar = kullaniciRepo.GetAll();
                ViewBag.Roller = rolRepo.GetAll()
                    .AsEnumerable()
                    .Select(x => new SelectListItem
                    {
                        Value = x.Id.ToString(),
                        Text = TurkishTextNormalizer.NormalizeDisplayText(x.RolAdi)
                    })
                    .ToList();

                KullaniciRol kullaniciRolM = kullaniciRolRepo.FindBy(e => e.KullaniciId == kullaniciRol.KullaniciId).FirstOrDefault();
                if (kullaniciRolM != null)
                {
                    ViewBag.RecordResult = 3;
                    return RedirectToAction("Index", "Yetki");
                }
                else
                {
                    Kullanici kullanici = (Kullanici)Session["CurrentUser"];
                    kullaniciRolM = new KullaniciRol();
                    kullaniciRolM.KayitTarihi = DateTime.Now;
                    kullaniciRolM.RolId = kullaniciRol.RolId;
                    kullaniciRolM.KullaniciId = kullaniciRol.KullaniciId;
                    kullaniciRolM.EkleyenKullaniciId = kullanici.Id;
                    kullaniciRolRepo.AddAndSave(kullaniciRolM);
                    ModelState.Clear();

                    return RedirectToAction("Index", "Yetki");
                }
            }
            return RedirectToAction("Index", "Yetki");
        }

        [HttpGet]
        [AuthLog(Roles = "DUZENLEME")]
        public ActionResult KullaniciRolDuzenleme(int Id)
        {
            TempData["loader"] = "Lütfen bekleyiniz...";
            kullaniciRolRepo = new KullaniciRolRepo();
            kullaniciRepo = new KullaniciRepo();
            rolRepo = new RolRepo();
            KullaniciRol kullaniciRol = kullaniciRolRepo.FindBy(e => e.Id == Id).FirstOrDefault();

            List<Kullanici> kullaniciList = kullaniciRepo.GetAll().ToList();
            List<Rol> rolList = rolRepo.GetAll().ToList();
            
            List<SelectListItem> kullaniciSelectList = kullaniciList.ConvertAll(a =>
            {
                return new SelectListItem()
                {
                    Text = a.KullaniciAdSoyadMail,
                    Value = a.Id.ToString(),
                    Selected = false
                };
            });
            var seciliKullanici = kullaniciSelectList.Where(x => x.Value.Equals(kullaniciRol.KullaniciId.ToString())).FirstOrDefault();
            seciliKullanici.Selected = true;

            List<SelectListItem> rolSelectList = rolList.ConvertAll(a =>
            {
                return new SelectListItem()
                {
                    Text = TurkishTextNormalizer.NormalizeDisplayText(a.RolAdi),
                    Value = a.Id.ToString(),
                    Selected = false
                };
            });
            var seciliRol = rolSelectList.Where(x => x.Value.Equals(kullaniciRol.RolId.ToString())).FirstOrDefault();
            seciliRol.Selected = true;

            ViewBag.Kullanicilar = kullaniciSelectList;
            ViewBag.Roller = rolSelectList;
            return PartialView("KullaniciRolDuzenleme");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthLog(Roles = "DUZENLEME")]
        public ActionResult KullaniciRolDuzenleme(KullaniciRol kullaniciRol)
        {
            kullaniciRolRepo = new KullaniciRolRepo();
            KullaniciRol kullaniciRols = kullaniciRolRepo.FindBy(e => e.Id == kullaniciRol.Id).FirstOrDefault();
            kullaniciRols.DuzenlemeTarihi = DateTime.Now;
            kullaniciRols.RolId = kullaniciRol.RolId;

            kullaniciRolRepo.EditAndSave(kullaniciRols);

            return RedirectToAction("Index", "Yetki");
        }
    }
}
