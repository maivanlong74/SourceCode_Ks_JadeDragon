/*using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Jade_Dragon.Areas.Admin.Controllers;
using Jade_Dragon.Models;

namespace Jade_Dragon.Controllers
{
    public class nhanxetController : Controller
    {
        private Connect db = new Connect();

        // GET: nhanxet
        public ActionResult nhanxet(int? page)
        {
            int pageSize = 12; // số lượng phần tử hiển thị trong mỗi trang
            int pageNumber = (page ?? 1); // trang hiện tại, mặc định là trang đầu tiên

            var NhanXet = db.phanhois.Include(k => k.khachhang).OrderByDescending(x => x.ThoiGian);
            int totalItems = NhanXet.Count();
            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            NhanXet = (IOrderedQueryable<phanhoi>)NhanXet.Skip((pageNumber - 1) * pageSize).Take(pageSize);
            ViewBag.TotalItems = totalItems;
            ViewBag.TotalPages = totalPages;
            ViewBag.PageNumber = pageNumber;
            ViewBag.PageSize = pageSize;
            var list = NhanXet.ToList();
            var listkh = db.khachhangs.Select(kh => kh.MaKh).ToList();

            foreach (var item in list)
            {
                if (item.MaKh != null && !listkh.Contains(item.MaKh.Value))
                {
                    item.MaKh = null;
                }
            }
            db.SaveChanges();



            return View("nhanxet", list);
        }

        public ActionResult Create([Bind(Include = "MaPhanHoi,MaKh,NoiDung,ThoiGian")] phanhoi PhanHoi, long MaKh)
        {
            if (Request.Cookies["MaKh"] != null)
            {
                MaKh = long.Parse(Request.Cookies["MaKh"].Value);
            }
            if (db.khachhangs.Where(a => a.MaKh == MaKh).Count() > 0)
            {
                    PhanHoi.MaPhanHoi = RandomChuCai() + RandomChuSo();
                    PhanHoi.ThoiGian = DateTime.Now;
                if (!db.phanhois.Any(m => m.MaPhanHoi == PhanHoi.MaPhanHoi))
                {
                    db.phanhois.Add(PhanHoi);
                    db.SaveChanges();
                    return Redirect("nhanxet");
                }
            }
            return Redirect("nhanxet");
        }

        // GET: nhanxet/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            phanhoi phanhoi = db.phanhois.Find(id);
            if (phanhoi == null)
            {
                return HttpNotFound();
            }
            ViewBag.MaKh = new SelectList(db.khachhangs, "MaKh", "HoTen", phanhoi.MaKh);
            return View(phanhoi);
        }

        // POST: nhanxet/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MaTinNhan,MaKh,NoiDung")] phanhoi phanhoi)
        {
            if (ModelState.IsValid)
            {
                db.Entry(phanhoi).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.MaKh = new SelectList(db.khachhangs, "MaKh", "HoTen", phanhoi.MaKh);
            return View(phanhoi);
        }

        // GET: nhanxet/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            phanhoi phanhoi = db.phanhois.Find(id);
            if (phanhoi == null)
            {
                return HttpNotFound();
            }
            return View(phanhoi);
        }

        // POST: nhanxet/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            phanhoi phanhoi = db.phanhois.Find(id);
            db.phanhois.Remove(phanhoi);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public static string RandomChuCai()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            Random random = new Random();
            return new string(Enumerable.Repeat(chars, 3)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public string RandomChuSo()
        {
            int codeLength = 2; // Độ dài của mã xác minh
            Random random = new Random();
            string code = "";

            for (int i = 0; i < codeLength; i++)
            {
                int digit = random.Next(0, 9); // Lấy ngẫu nhiên một số từ 0 đến 9
                code += digit.ToString(); // Thêm số vào chuỗi mã xác minh
            }

            return code;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
*/