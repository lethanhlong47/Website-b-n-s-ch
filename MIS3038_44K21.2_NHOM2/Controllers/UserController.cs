using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using WebBookStore.Common;
using WebBookStore.Models;

namespace WebBookStore.Controllers
{
    public class UserController : Controller
    {
        // GET: User
        public ActionResult Index()
        {
            string UserFullName = UserDao.Instance.GetUserFullName(UserDao.Instance.GetUserId());
            ViewBag.UserFullName = UserFullName;
            var user = UserDao.Instance.ViewDetails(UserDao.Instance.GetUserId());
            return View(user);
        }

        public ActionResult Edit()
        {
            string UserFullName = UserDao.Instance.GetUserFullName(UserDao.Instance.GetUserId());
            ViewBag.UserFullName = UserFullName;
            var user = UserDao.Instance.ViewDetails(UserDao.Instance.GetUserId());
            return View(user);
        }

        [HttpPost]
        public ActionResult Edit(Account user)
        {
            if (ModelState.IsValid)
            {
                var result = UserDao.Instance.Update(user);
                if (result)
                {
                    ModelState.AddModelError("", "* Cập nhật thành công!");
                    return RedirectToAction("Index", "User");
                }
                else
                {
                    ViewBag.Fail = "Cap nhat khong thanh cong";
                    ModelState.AddModelError("", "* Cập nhật không thành công!");
                }
            }
            return View();
        }
        // GET: NewAdmin1/User
        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register([Bind(Exclude = "ActivationCode")] Register model)
        {
            bool Status = false;
            string Message = "";

            //Model Validation
            if (ModelState.IsValid)
            {
                //username already exists
                if (UserDao.Instance.CheckUserName(model.UserName))
                {
                    ModelState.AddModelError("", "*Tên đăng nhập tồn tại!");
                }
                //email already exists
                else if (UserDao.Instance.CheckUserEmail(model.Email))
                {
                    ModelState.AddModelError("", "*Email đã tồn tại !");
                }
                else
                {
                    var user = new Account();

                    model.ActivationCode = Guid.NewGuid();
                    user.ActivationCode = model.ActivationCode;
                    user.UserName = model.UserName;
                    user.Password = Encryptor.MD5Hash(model.Password);
                    user.Email = model.Email;
                   
                    user.Name = model.Name;
                    user.Access = true;
                    user.CreatedDate = DateTime.Now;
                    user.GroupID = "CUSTOMER";
                    var result = UserDao.Instance.Insert(user);
                    if (result > 0)
                    {
                       
                        Message = "Đăng ký thành công. Tài khoản đã được kích hoạt" ;
                        Status = true;
                        model = new Register();
                    }
                    else
                    {
                        Message = "Invalid Request";
                    }
                }
            }
            ViewBag.Message = Message;
            ViewBag.Status = Status;
            return View(model);
        }

        
        [HttpGet]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ForgotPassword(string Email)
        {
            //verify email
            ////generate reset password link
            //send email
            string message = "";
            bool status = false;
            var user = UserDao.Instance.GetUserByEmail(Email);
            if (user != null)
            {
                //send email for reset pass
                string resetCode = Guid.NewGuid().ToString();
                
                user.ResetPasswordCode = resetCode;
                //this line i have added here to avoid confirm password not match issue, as we had addded a confirm password property
                //in our model class Account
                UserDao.Instance.ValidateOnSaveEnabled();
                UserDao.Instance.SaveChanges();
                status = true;
                message = "Kiểm tra Email để thay đổi mật khẩu!";
            }
            else
            {
                message = "* Tài khoản Email không tìm thấy!";
            }
            ViewBag.Status = status;
            ViewBag.Message = message;
            return View();
        }
        [HttpGet]
        public ActionResult ResetPassword(string id)
        {
            //verify the reset password link
            //find account associated with this link
            //redirect to view reset pass word
            if (UserDao.Instance.GetUserByResetCode(id) != null)
            {
                ResetPasswordModel model = new ResetPasswordModel();
                model.ResetCode = id;
                return View(model);
            }
            else
            {
                return HttpNotFound();
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword(ResetPasswordModel  model)
        {
            var message = "";
            if (ModelState.IsValid)
            {
                var user = UserDao.Instance.GetUserByResetCode(model.ResetCode);
                if (user != null)
                {
                    user.Password = Encryptor.MD5Hash(model.NewPassword);
                    user.ResetPasswordCode = "";
                    UserDao.Instance.ValidateOnSaveEnabled();
                    UserDao.Instance.SaveChanges();
                    message = "* Mật khẩu mới đã được cập nhật!";
                }
            }
            else
            {
                message = "* Mật khẩu cập nhật không thành công!!";
            }
            ViewBag.Message = message;
            return View(model);
        }
    }
}