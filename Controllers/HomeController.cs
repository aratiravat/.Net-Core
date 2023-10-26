using Quiz.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList.Mvc;
using PagedList;

namespace Quiz.Controllers
{
    public class HomeController : Controller
    {
        //use db for add new category
        DBQUIZEntities db = new DBQUIZEntities();

        score sc = new score();//ye abhi banaya hau ibject class ka 

        int scoreExam = 0;

      
        public ActionResult tlogin()
        {
            return View();
        }

        [HttpPost]
        public ActionResult tlogin(TBL_ADMIN a)
        {
            //to login with currect credentials

            TBL_ADMIN ad = db.TBL_ADMIN.Where(x => x.AD_NAME == a.AD_NAME && x.AD_PASSWORD == a.AD_PASSWORD).SingleOrDefault();
            if (ad != null)
            {
                //after login create session
                Session["AD_ID"] = ad.AD_ID;
                return RedirectToAction("Dashboard");
            }
            else
            {
                ViewBag.msg = "Invalid username or password....";
            }
            return View();
        }

        //logout for tlogin
        public ActionResult Logout()
        {
            Session.Abandon();
            Session.RemoveAll();

            return RedirectToAction("Index");
        }

        //student register

        [HttpGet]
        public ActionResult sregister()
        {

            return View();
        }

        //post request for registeration
        [HttpPost]
        public ActionResult sregister(TBL_STUDENT svw, HttpPostedFileBase imgfile)
        {
            TBL_STUDENT s = new TBL_STUDENT();
            try
            {
                s.S_NAME = svw.S_NAME;
                s.S_PASSWORD = svw.S_PASSWORD;
                s.S_IMAGE = uploadimage(imgfile);
                db.TBL_STUDENT.Add(s);
                db.SaveChanges();
                return RedirectToAction("slogin");
            }
            catch (Exception)
            {
                ViewBag.msg = "Data could not be inserted...";
            }
            return View();
        }

        //for uploading image
        public string uploadimage(HttpPostedFileBase imgfile)
        {
            string path = "-1";

            try
            {

                if (imgfile != null && imgfile.ContentLength > 0)
                {
                    /*
                    string extension = Path.GetExtension(imgfile.FileName);
                    if (extension.ToLower().Equals("jpg") || extension.ToLower().Equals("jpeg") || extension.ToLower().Equals("png"))
                    {
                        //Path.Combine(Server.MapPath("~/Content/upload"), random + Path.GetFileName(file.FileName));

                        Random r = new Random();
                        path = Path.Combine(Server.MapPath("~/Content/img"), r + Path.GetFileName(imgfile.FileName));
                        imgfile.SaveAs(path);
                        path = ("~/Content/img" + r + Path.GetFileName(imgfile.FileName));
                    }
                    */
                    string extension = Path.GetExtension(imgfile.FileName);
                    if (extension.Equals(".jpg", StringComparison.OrdinalIgnoreCase) ||
                        extension.Equals(".jpeg", StringComparison.OrdinalIgnoreCase) ||
                        extension.Equals(".png", StringComparison.OrdinalIgnoreCase))
                    {
                        Random r = new Random();
                        string fileName = Path.GetFileName(imgfile.FileName);
                        string uniqueFileName = r.Next() + "_" + fileName;
                        path = Path.Combine(Server.MapPath("~/Content/img"), uniqueFileName);
                        imgfile.SaveAs(path);
                        path = "~/Content/img/" + uniqueFileName;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            return path;
        }
        public ActionResult slogin()
        {
            return View();
        }

        [HttpPost]
        public ActionResult slogin(TBL_STUDENT s)
        {

            TBL_STUDENT std = db.TBL_STUDENT.Where(x => x.S_NAME == s.S_NAME && x.S_PASSWORD == s.S_PASSWORD).SingleOrDefault();

            sc.scoree[0] = std.S_ID.ToString();
            sc.scoree[1] = std.S_NAME;


            if (std == null)
            {
                ViewBag.msg = "Invalid Email or Password";
            }
            else
            {
                Session["std_id"] = std.S_ID;
                return RedirectToAction("StudentDashboard");
            }
            return View();
        }

        public ActionResult StudentDashboard()
        {
            if (Session["std_id"] == null)
            {
                return RedirectToAction("slogin");
            }
            return View();
        }

       
        //set exam after login
        public ActionResult StudentExam()
        {
            if (Session["std_id"] == null)
            {
                return RedirectToAction("slogin");
            }

            return View();
        }

        /*
        [HttpPost]
        public ActionResult StudentExam(string room)
        {
            List<TBL_CATEGORY> list = db.TBL_CATEGORY.ToList();



            foreach (var item in list)
            {
                if (item.CAT_ENCYPTEDSTRING == room)
                {
                    TempData["exampid"] = item.CAT_ID;
                    TempData.Keep();
                    return RedirectToAction("QuizStart");
                }
                else
                {
                    ViewBag.error = "No Room Found............";
                }
            }
            return View();
        }
    */

        
        [HttpPost]
        public ActionResult StudentExam(string room)
        {
            //take list from tbl_category
            List<TBL_CATEGORY> list = db.TBL_CATEGORY.ToList();

            foreach (var item in list)
            {
                if (item.CAT_ENCYPTEDSTRING == room)
                {

                    //part 8
                    //store li
                    List<TBL_QUESTIONS> li = db.TBL_QUESTIONS.Where(x => x.Q_FK_CATID == item.CAT_ID).ToList();
                    Queue<TBL_QUESTIONS> queue = new Queue<TBL_QUESTIONS>();
                    foreach (TBL_QUESTIONS a in li)
                    {
                        queue.Enqueue(a);
                        
                    }
                   
                    TempData["questions"] = queue;
                    //TempData["Score"] = 0;


                    //part8 - TempData["exampid"] = item.CAT_ID;
                    TempData.Keep();
                    return RedirectToAction("QuizStart");
                }
                else
                {
                    ViewBag.error = "No Room Found............";
                }
            }
            return View();
        }

        

        /*
        public ActionResult QuizStart()
        {
           

            if (Session["std_id"] == null)
            {
                return RedirectToAction("slogin");
            }

           
                TBL_QUESTIONS q = null;
                int examid = Convert.ToInt32(TempData["exampid"].ToString());

                if (TempData["QUESTIONS_ID"] == null)
                {

                    q = db.TBL_QUESTIONS.FirstOrDefault(x => x.Q_FK_CATID == examid);
                    //TBL_QUESTIONS q = db.TBL_QUESTIONS.Where(x => x.Q_FK_CATID == examid);
                    //var list = db.TBL_QUESTIONS.Skip(Convert.ToInt32(TempData["i"].ToString()));
                    //int qid = list.FirstOrDefault().QUESTIONS_ID;
                    //TempData["QUESTIONS_ID"] = qid;
                    TempData["QUESTIONS_ID"] = ++q.QUESTIONS_ID;
                }
                else
                {
                    int qid = Convert.ToInt32(TempData["QUESTIONS_ID"].ToString());
                    q = db.TBL_QUESTIONS.Where(x => x.QUESTIONS_ID == qid && x.Q_FK_CATID == examid).SingleOrDefault();
                   // var list = db.TBL_QUESTIONS.Skip(Convert.ToInt32(TempData["i"].ToString()));
                   // qid = list.First().QUESTIONS_ID;
                   // TempData["QUESTIONS_ID"] = qid;

                  //  TempData["i"] = Convert.ToInt32(TempData["i"].ToString()) + 1;

                    TempData["QUESTIONS_ID"] = ++q.QUESTIONS_ID;
                
                }
                TempData.Keep();
                return View(q);
           
        }

     */ 

        public ActionResult QuizStart()
        {
            if (Session["std_id"] == null)
            {
                return RedirectToAction("slogin");
            }

            TBL_QUESTIONS q = null;

            //if question list is not null then fetch it
            if (TempData["questions"] != null)
            {
                Queue<TBL_QUESTIONS> qlist = (Queue<TBL_QUESTIONS>)TempData["questions"];

                if (qlist.Count > 0)
                {
                    q = qlist.Peek();
                    qlist.Dequeue();
                    TempData["questions"] = qlist;
                    TempData.Keep();
                }
                else
                {
                    return RedirectToAction("EndExam");
                }
            }
            else
            {
                return RedirectToAction("StudentExam");
            }
            TempData.Keep();
            return View(q);

        }
       
        
        [HttpPost]
        public ActionResult QuizStart(TBL_QUESTIONS q)
        {
           String correctAns = null;

            if (q.OPA != null)
            {
                /*
                if (q.OPA.Equals(q.COP))
                {
                    TempData["Score"] = Convert.ToInt32(TempData["Score"] )+ 1;
                }
                */
                correctAns = "A";
              
            }
            else if(q.OPB != null)
            {
                /*
                if (q.OPB.Equals(q.COP))
                {
                    TempData["Score"] = Convert.ToInt32(TempData["Score"]) + 1;
                }
                */
                 correctAns = "B";
            }
            else if(q.OPC != null)
            {
                /*
                if (q.OPC.Equals(q.COP))
                {
                    TempData["Score"] = Convert.ToInt32(TempData["Score"]) + 1;
                }
                */
                correctAns = "C";
            }
           else if (q.OPD != null)
            {
                /*
                if (q.OPD.Equals(q.COP))
                {
                    TempData["Score"] = Convert.ToInt32(TempData["Score"]) + 1;
                }
                */
                 correctAns = "D";
            }


          

            if (correctAns.Equals(q.COP))
            {
                scoreExam = scoreExam + 1;
            }

           

            TempData["ScoreExam"] = scoreExam;
            TempData.Keep();

            return RedirectToAction("QuizStart");
        }


            public ActionResult Dashboard()
        {
            if (Session["AD_ID"] == null)
            {
                return RedirectToAction("Index");
            }
            return View();
        }


        public ActionResult ViewAllQuestions(int? id, int? page)
        {
            if (Session["AD_ID"] == null)
            {
                return RedirectToAction("tlogin");
            }

            if (id == null)
            {
                return RedirectToAction("Dashboard");
            }

            int pagesize = 15, pageindex = 1;
            pageindex = page.HasValue ? Convert.ToInt32(page) : 1;

            List<TBL_QUESTIONS> li = db.TBL_QUESTIONS.Where(x => x.Q_FK_CATID == id).ToList();
            IPagedList<TBL_QUESTIONS> stu = li.ToPagedList(pageindex, pagesize);

            return View();



        }

        //endexam
        public ActionResult EndExam(TBL_QUESTIONS q)
        {
            if (Session["std_id"] == null)
            {
                return RedirectToAction("slogin");
            }

            TempData["ScoreExam"] = 2;
            TempData.Keep();
            return View();
        }



        /*
        public ActionResult records(int? page)
        {
            if (Session["AD_ID"] == null)
            {
                return RedirectToAction("Index");
            }
            score sc = new score();
            List<score> li = sc.getreport(Convert.ToInt32(Session["AD_ID"]));

            int pagesize = 15, pageindex = 1;
            pageindex = page.HasValue ? Convert.ToInt32(page) : 1;

            IPagedList<score> stu = li.ToPagedList(pageindex, pagesize);

           return View(stu);
        }
        */

        //call the hhtp request to add category
        [HttpGet]
        public ActionResult Addcategory()
        {
            //hard coded session id just for now
            //Session["AD_ID"] = 1;
            if(Session["AD_ID"] == null)
            {
                return RedirectToAction("Index");
            }
            int adid = Convert.ToInt32(Session["AD_ID"].ToString());

            List<TBL_CATEGORY> li = db.TBL_CATEGORY.Where(x => x.CAT_FK_ADID == adid).OrderByDescending(x => x.CAT_ID).ToList();
            ViewData["list"] = li;
            return View();
        }

        [HttpPost]
        public ActionResult Addcategory(TBL_CATEGORY cat)
        {
            //hard coded session id just for now
            //Session["AD_ID"] = 1;
            if (Session["AD_ID"] == null)
            {
                return RedirectToAction("Index");
            }

            List<TBL_CATEGORY> li = db.TBL_CATEGORY.OrderByDescending(x => x.CAT_ID).ToList();
            ViewData["list"] = li;

            Random r = new Random();
            //for session
            TBL_CATEGORY c = new TBL_CATEGORY();
            c.CAT_NAME = cat.CAT_NAME;
            //create the class code encrypted, encript is method inside crypto.cs(models)
            c.CAT_ENCYPTEDSTRING =crypto.Encrypt(cat.CAT_NAME.Trim() + r.Next().ToString(), true);
            c.CAT_FK_ADID = Convert.ToInt32(Session["AD_ID"].ToString());

            db.TBL_CATEGORY.Add(c);
            db.SaveChanges();

            return RedirectToAction("Addcategory");
        }


        //now for adding the questions
        [HttpGet]
        public ActionResult AddQuestions()
        {
            // Session["AD_ID"] = 1;
            int sid = Convert.ToInt32(Session["AD_ID"]);

            //this is how we can add the select dropdown menu where we see the added subjevts category
            List<TBL_CATEGORY> li = db.TBL_CATEGORY.Where(x => x.CAT_FK_ADID == sid).ToList();
            ViewBag.list = new SelectList(li, "CAT_ID", "CAT_NAME");
            return View();
        }

        //post request to add questions
        [HttpPost]
        public ActionResult AddQuestions(TBL_QUESTIONS q)
        {
            if (Session["AD_ID"] == null)
            {
                return RedirectToAction("Index");
            }
            // Session["AD_ID"] = 1;
            int sid = Convert.ToInt32(Session["AD_ID"]);

            //this is how we can add the select dropdown menu where we see the added subjevts category
            List<TBL_CATEGORY> li = db.TBL_CATEGORY.Where(x => x.CAT_FK_ADID == sid).ToList();
            ViewBag.list = new SelectList(li, "CAT_ID", "CAT_NAME");

            //for session
            TBL_QUESTIONS qa = new TBL_QUESTIONS();
           
            qa.Q_TEXT = q.Q_TEXT;
            qa.OPA = q.OPA;
            qa.OPB = q.OPB;
            qa.OPC = q.OPC;
            qa.OPD = q.OPD;
            qa.COP = q.COP;
            qa.Q_FK_CATID = q.Q_FK_CATID;

            db.TBL_QUESTIONS.Add(qa);
            db.SaveChanges();
           // ViewBag.msg = "Questions added...";

            TempData["msg"] = "Question added successfully..... ";
            TempData.Keep();
            return RedirectToAction("AddQuestions");

           // return View();
        }
        public ActionResult Index()
        {
            if (Session["ad id"] != null)
            {
                return RedirectToAction("Dashboard");

            }
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}