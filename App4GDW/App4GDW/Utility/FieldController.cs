using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using GolfWorld1.Models;
using GolfWorld1.Util;
using Microsoft.AspNet.Identity;
using PagedList;

namespace GolfWorld1.Controllers
{
    public class FieldController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        private int HolePageSize = 1;
        private const int FirstPageIndex = 1;
        private int? _fullCategoryCount;

        private HoleScore HoleScore { get; set; }
        private List<HoleScore> HoleScores { get; set; }
        private Dictionary<int, HoleScore> ListHoleScores { get; set; }


        #region Private Methods

        private int getLastPageIndex()
        {
            //int lastPageIndex = this.FullCategoryCount / PageSize;
            //if (this.FullCategoryCount % PageSize > 0)
            //    lastPageIndex += 1;

            return 18;
        }

        public FieldController()
        {
            ListHoleScores = new Dictionary<int, HoleScore>();
        }

        #endregion

        //// GET: Field
        //public ActionResult Index()
        //{
        //    return View();
        //}

        // GET: Field/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Field/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Field/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Field/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Field/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Field/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Field/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        //
        //
        //
        // POST: Field/Index
        // Step 1
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.NameSortParam = sortOrder == "name" ? "name_desc" : "name";
            ViewBag.AddressSortParam = sortOrder == "address" ? "address_desc" : "address";
            ViewBag.DateSortParam = sortOrder == "date" ? "date_desc" : "date";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            //ViewBag.CurrentSort = sortOrder;

            var courses = from c in db.Courses select c;

            if (!String.IsNullOrEmpty(searchString))
            {
                courses = courses.Where(s => s.Name.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "name":
                    courses = courses.OrderBy(x => x.Name);
                    break;
                case "address":
                    courses = courses.OrderBy(x => x.Address);
                    break;
                case "address_desc":
                    courses = courses.OrderByDescending(x => x.Address);
                    break;
                case "city":
                    courses = courses.OrderBy(x => x.City);
                    break;
                case "city_desc":
                    courses = courses.OrderByDescending(x => x.City);
                    break;
                default:
                    break;
            }

            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(courses.OrderBy(i => i.Name).ToPagedList(pageNumber, pageSize));
        }

        //
        // GET: Field/FieldSelectTee
        // Step 2a
        public ActionResult FieldSelectTee(int id)
        {
            Course course = db.Courses.Find(id);
            //SelectList tees = new SelectList();
            List<SelectListItem> tees = new List<SelectListItem>();

            foreach (var a in course.TCInfos)
            {
                if (!string.IsNullOrEmpty(a.Name))
                {
                    tees.Add(new SelectListItem() { Value = a.Name, Text = a.Name });
                }
            }

            ViewData["teeNames"] = tees;

            return View();
        }

        // POST: Field/FieldSelectTee
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        // Step 2b
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult FieldSelectTee(int? id, string gender, string teeName, DateTime playStartDateTime, int nineHole)
        {
            Course course = db.Courses.Find(id);

            // create top 3 rows for scorecard
            TeeCommonInfo tciPar =
                (from a in course.TCInfos where a.Gender == gender where a.Theme == "Par" select a).FirstOrDefault();

            TeeCommonInfo tciHandicap =
                (from a in course.TCInfos where a.Gender == gender where a.Theme == "Handicap" select a).FirstOrDefault();

            TeeCommonInfo tciDistance =
                (from a in course.TCInfos where a.Gender == gender where a.Theme == "Distance" where a.Name == teeName select a).FirstOrDefault();

            // create a new empty round so that we can have a roundID
            Round round = new Round() { RecordDate = DateTime.Now, PlayDate = playStartDateTime, PlayStartTime = playStartDateTime, PlayEndTime = playStartDateTime.AddHours(4.5), CourseID = course.GCID, TeeID = tciDistance.TCID, NineHole = nineHole };

            round.Course = course;

            //string aaa = User.Identity.GetUserId();

            round.UserGUID = Guid.Parse(User.Identity.GetUserId());
            round.UserName = User.Identity.Name;

            // to get a roundID
            // save partial info to round, later once a user completed entering score, then fully save it
            // todo: do I have to save to get the RID? what if I save both round and scorecard at the end of enterScoreCard and save them at the same time?
            db.Rounds.Add(round);
            db.SaveChanges();

            // ??? here ???
            // do I have to do this?
            course.Rounds.Add(round);

            // create all the score card rows
            ScoreCard sc0 = new ScoreCard()
            {
                CreateDate = DateTime.Today,
                UpdateDate = DateTime.Today,
                FRID = round.RID,
                FTCID = tciPar.TCID,
                RowNumber = 0,
                Theme = "Par"
            };
            ScoreCard sc1 = new ScoreCard()
            {
                CreateDate = DateTime.Today,
                UpdateDate = DateTime.Today,
                FRID = round.RID,
                FTCID = tciHandicap.TCID,
                RowNumber = 1,
                Theme = "Handicap"
            };
            ScoreCard sc2 = new ScoreCard()
            {
                CreateDate = DateTime.Today,
                UpdateDate = DateTime.Today,
                FRID = round.RID,
                FTCID = tciDistance.TCID,
                RowNumber = 2,
                Theme = "Distance"
            };

            CopyPropertiesH(tciPar, sc0);
            CopyPropertiesH(tciHandicap, sc1);
            CopyPropertiesH(tciDistance, sc2);

            sc0.HOut = tciPar.HOut;
            sc0.HIn = tciPar.HIn;
            sc0.HTotal = tciPar.HTotal;
            sc1.HOut = tciHandicap.HOut;
            sc1.HIn = tciHandicap.HIn;
            sc1.HTotal = tciHandicap.HTotal;
            sc2.HOut = tciDistance.HOut;
            sc2.HIn = tciDistance.HIn;
            sc2.HTotal = tciDistance.HTotal;

            List<ScoreCard> scoreCards = new List<ScoreCard>();
            scoreCards.Add(sc0);
            scoreCards.Add(sc1);
            scoreCards.Add(sc2);

            scoreCards.Add(new ScoreCard() { RowNumber = 3, Theme = "Score" });
            scoreCards.Add(new ScoreCard() { RowNumber = 4, Theme = "AltScore" });
            scoreCards.Add(new ScoreCard() { RowNumber = 5, Theme = "Putt" });
            scoreCards.Add(new ScoreCard() { RowNumber = 6, Theme = "Fairway" });
            scoreCards.Add(new ScoreCard() { RowNumber = 7, Theme = "GIR" });
            scoreCards.Add(new ScoreCard() { RowNumber = 8, Theme = "Pitch" });
            scoreCards.Add(new ScoreCard() { RowNumber = 9, Theme = "Chip" });
            scoreCards.Add(new ScoreCard() { RowNumber = 10, Theme = "GSBunker" });
            scoreCards.Add(new ScoreCard() { RowNumber = 11, Theme = "Penalty" });

            // now add scoreCards to round.ScoreCards
            round.ScoreCards = scoreCards;

            // kpkp: do I have to save round with empty score card here for the later use???
            foreach (ScoreCard score in round.ScoreCards)
            {
                score.FRID = round.RID;
                db.ScoreCards.Add(score);
            }

            db.SaveChanges();

            //TempData["Round"] = round;

            // pass only round
            ViewBag.Title = round.Course.Name + "," + teeName;

            if (ModelState.IsValid)
            {
                return RedirectToAction("FieldEnterScoreHole", "Field", new {id=round.RID, ppage = 1, cpage = 1, 
                    hScore = 0, hAltScore = 0, hPutt = 0, hFairway = 0,
                    hGir = 0, hPitch = 0, hChip = 0, hGsBunker = 0, hPenalty = 0});
            }

            return View();
        }

        //
        // GET: Field/FieldEnterScoreHole
        // Step 2a
        // don't need to pass these three: string hPar, string hHandi, string hDistance, 
        public ActionResult FieldEnterScoreHole(int id, int ppage, int cpage, string hScore, 
            string hAltScore, string hPutt, string hFairway, string hGir, string hPitch, string hChip, string hGsBunker, string hPenalty)
        {
            ModelState.Clear();
            ViewBag.CurrentPageIndex = FirstPageIndex;

            Round round = db.Rounds.Find(id);
            IList<ScoreCard> listScoreCards = db.ScoreCards.Where(s => s.FRID == round.RID).ToList();

            string holeName = ppage < 10 ? "H0" + ppage : "H" + ppage;

            int par = int.Parse(listScoreCards[0].GetType().GetProperties().FirstOrDefault(a => a.Name == holeName).GetValue(listScoreCards[0]).ToString());
            hAltScore = (int.Parse(hScore) - par).ToString();

            //listScoreCards[0].GetType().GetProperties().FirstOrDefault(a => a.Name == holeName).SetValue(listScoreCards[0], int.Parse(hPar));
            //listScoreCards[1].GetType().GetProperties().FirstOrDefault(a => a.Name == holeName).SetValue(listScoreCards[1], int.Parse(hHandi));
            //listScoreCards[2].GetType().GetProperties().FirstOrDefault(a => a.Name == holeName).SetValue(listScoreCards[2], int.Parse(hDistance));
            listScoreCards[3].GetType().GetProperties().FirstOrDefault(a => a.Name == holeName).SetValue(listScoreCards[3], int.Parse(hScore));
            listScoreCards[4].GetType().GetProperties().FirstOrDefault(a => a.Name == holeName).SetValue(listScoreCards[4], int.Parse(hAltScore));
            listScoreCards[5].GetType().GetProperties().FirstOrDefault(a => a.Name == holeName).SetValue(listScoreCards[5], int.Parse(hPutt));
            listScoreCards[6].GetType().GetProperties().FirstOrDefault(a => a.Name == holeName).SetValue(listScoreCards[6], int.Parse(hFairway));
            listScoreCards[7].GetType().GetProperties().FirstOrDefault(a => a.Name == holeName).SetValue(listScoreCards[7], int.Parse(hGir));
            listScoreCards[8].GetType().GetProperties().FirstOrDefault(a => a.Name == holeName).SetValue(listScoreCards[8], int.Parse(hPitch));
            listScoreCards[9].GetType().GetProperties().FirstOrDefault(a => a.Name == holeName).SetValue(listScoreCards[9], int.Parse(hChip));
            listScoreCards[10].GetType().GetProperties().FirstOrDefault(a => a.Name == holeName).SetValue(listScoreCards[10], int.Parse(hGsBunker));
            listScoreCards[11].GetType().GetProperties().FirstOrDefault(a => a.Name == holeName).SetValue(listScoreCards[11], int.Parse(hPenalty));

            cpage = cpage%18 == 0 ? 18 : cpage%18;

            string cHoleName = cpage < 10 ? "H0" + cpage : "H" + cpage;

            ViewBag.CurrentPage = cpage;
            ViewBag.PageSize = HolePageSize;
            ViewBag.TotalPages = 18;

            // test for the properties
            ViewBag.RID = round.RID;
            ViewBag.HolePar = listScoreCards[0].GetType().GetProperties().FirstOrDefault(a => a.Name == cHoleName).GetValue(listScoreCards[0]) ?? 0;
            ViewBag.HoleHandi = listScoreCards[1].GetType().GetProperties().FirstOrDefault(a => a.Name == cHoleName).GetValue(listScoreCards[1]) ?? 0;
            ViewBag.HoleDistance = listScoreCards[2].GetType().GetProperties().FirstOrDefault(a => a.Name == cHoleName).GetValue(listScoreCards[2]) ?? 0;
            ViewBag.HoleScore = listScoreCards[3].GetType().GetProperties().FirstOrDefault(a => a.Name == cHoleName).GetValue(listScoreCards[3]) ?? 0;
            ViewBag.HoleAltScore = listScoreCards[4].GetType().GetProperties().FirstOrDefault(a => a.Name == cHoleName).GetValue(listScoreCards[4]) ?? 0;
            ViewBag.HolePutt = listScoreCards[5].GetType().GetProperties().FirstOrDefault(a => a.Name == cHoleName).GetValue(listScoreCards[5]) ?? 0;
            ViewBag.HoleFairway = listScoreCards[6].GetType().GetProperties().FirstOrDefault(a => a.Name == cHoleName).GetValue(listScoreCards[6]) ?? 0;
            ViewBag.HoleGir = listScoreCards[7].GetType().GetProperties().FirstOrDefault(a => a.Name == cHoleName).GetValue(listScoreCards[7]) ?? 0;
            ViewBag.HolePitch = listScoreCards[8].GetType().GetProperties().FirstOrDefault(a => a.Name == cHoleName).GetValue(listScoreCards[8]) ?? 0;
            ViewBag.HoleChip = listScoreCards[9].GetType().GetProperties().FirstOrDefault(a => a.Name == cHoleName).GetValue(listScoreCards[9]) ?? 0;
            ViewBag.HoleGsBunker = listScoreCards[10].GetType().GetProperties().FirstOrDefault(a => a.Name == cHoleName).GetValue(listScoreCards[10]) ?? 0;
            ViewBag.HolePenalty = listScoreCards[11].GetType().GetProperties().FirstOrDefault(a => a.Name == cHoleName).GetValue(listScoreCards[11]) ?? 0;

            List<SelectListItem> scoreNumbers = new List<SelectListItem>();
            List<SelectListItem> puttNumbers = new List<SelectListItem>();
            List<SelectListItem> fairwayNumbers = new List<SelectListItem>();
            List<SelectListItem> girNumbers = new List<SelectListItem>();
            List<SelectListItem> pitchNumbers = new List<SelectListItem>();
            List<SelectListItem> chipNumbers = new List<SelectListItem>();
            List<SelectListItem> gsBunkerNumbers = new List<SelectListItem>();
            List<SelectListItem> penaltyNumbers = new List<SelectListItem>();
            bool selectedScore = false;
            bool selectedPutt = false;
            bool selectedFairway = false;
            bool selectedGir = false;
            bool selectedPitch = false;
            bool selectedChip = false;
            bool selectedGsBunker = false;
            bool selectedPenalty = false;

            for (int i = 0; i < 13; i++)
            {
                //selectedScore = (i == int.Parse(ViewBag.HoleScore.ToString()));
                selectedScore = ViewBag.HoleScore == 0 ? (i == ViewBag.HolePar) : (i == int.Parse(ViewBag.HoleScore.ToString()));
                selectedPutt = (i == int.Parse(ViewBag.HolePutt.ToString()));

                scoreNumbers.Add(new SelectListItem() {Value = i.ToString(), Text = i.ToString(), Selected = selectedScore});
                puttNumbers.Add(new SelectListItem() {Value = i.ToString(), Text = i.ToString(), Selected = selectedPutt});
            }

            for (int i = 0; i < 5; i++)
            {
                selectedFairway = (i == int.Parse(ViewBag.HoleFairway.ToString()));
                selectedPitch = (i == int.Parse(ViewBag.HolePitch.ToString()));
                selectedChip = (i == int.Parse(ViewBag.HoleChip.ToString()));
                selectedGsBunker = (i == int.Parse(ViewBag.HoleGsBunker.ToString()));
                selectedPenalty = (i == int.Parse(ViewBag.HolePenalty.ToString()));

                fairwayNumbers.Add(new SelectListItem() { Value = i.ToString(), Text = i.ToString(), Selected = selectedFairway });
                pitchNumbers.Add(new SelectListItem() { Value = i.ToString(), Text = i.ToString(), Selected = selectedPitch });
                chipNumbers.Add(new SelectListItem() { Value = i.ToString(), Text = i.ToString(), Selected = selectedChip });
                gsBunkerNumbers.Add(new SelectListItem() { Value = i.ToString(), Text = i.ToString(), Selected = selectedGsBunker });
                penaltyNumbers.Add(new SelectListItem() { Value = i.ToString(), Text = i.ToString(), Selected = selectedPenalty });
            }

            for (int i = 0; i < 2; i++)
            {
                selectedGir = (i == int.Parse(ViewBag.HoleGir.ToString()));

                girNumbers.Add(new SelectListItem() { Value = i.ToString(), Text = i.ToString(), Selected = selectedGir });
            }

            // calculate HOut, HIn, and HTotal for individual scoreCard
            // this is different from ScoreController, where HOut etc is calculated real time by javascript

            Calcs cc = new Calcs();

            for (int i = 0; i < 12; i++)
            {
                if (i >= 3)
                {
                    listScoreCards[i].HOut = cc.SumFront9(listScoreCards[i]);
                    listScoreCards[i].HIn = cc.SumBack9(listScoreCards[i]);
                    listScoreCards[i].HTotal = listScoreCards[i].HOut + listScoreCards[i].HIn;
                }
                listScoreCards[i].CreateDate = DateTime.Now;
                //listScoreCards[i].UpdateDate = DateTime.Now;
            }

            // we need to fill some columns of round for summary of the round
            round.Score = int.Parse(round.ScoreCards.Where(a => a.Theme == "Score").Select(b => b.HTotal).FirstOrDefault().ToString() == "" ? "0" : round.ScoreCards.Where(a => a.Theme == "Score").Select(b => b.HTotal).FirstOrDefault().ToString());
            round.Putt = int.Parse(round.ScoreCards.Where(a => a.Theme == "Putt").Select(b => b.HTotal).FirstOrDefault().ToString() == "" ? "0" : round.ScoreCards.Where(a => a.Theme == "Putt").Select(b => b.HTotal).FirstOrDefault().ToString());
            round.GIR = double.Parse(round.ScoreCards.Where(a => a.Theme == "GIR").Select(b => b.HTotal).FirstOrDefault().ToString() == "" ? "0" : round.ScoreCards.Where(a => a.Theme == "GIR").Select(b => b.HTotal).FirstOrDefault().ToString());
            round.FH = double.Parse(round.ScoreCards.Where(a => a.Theme == "Fairway").Select(b => b.HTotal).FirstOrDefault().ToString() == "" ? "0" : round.ScoreCards.Where(a => a.Theme == "Fairway").Select(b => b.HTotal).FirstOrDefault().ToString());

            round.OutScore = int.Parse(round.ScoreCards.Where(a => a.Theme == "Score").Select(b => b.HOut).FirstOrDefault().ToString() == "" ? "0" : round.ScoreCards.Where(a => a.Theme == "Score").Select(b => b.HOut).FirstOrDefault().ToString());
            round.OutPutt = int.Parse(round.ScoreCards.Where(a => a.Theme == "Putt").Select(b => b.HOut).FirstOrDefault().ToString() == "" ? "0" : round.ScoreCards.Where(a => a.Theme == "Putt").Select(b => b.HOut).FirstOrDefault().ToString());
            round.OutGIR = double.Parse(round.ScoreCards.Where(a => a.Theme == "GIR").Select(b => b.HOut).FirstOrDefault().ToString() == "" ? "0" : round.ScoreCards.Where(a => a.Theme == "GIR").Select(b => b.HOut).FirstOrDefault().ToString());
            round.OutFH = double.Parse(round.ScoreCards.Where(a => a.Theme == "Fairway").Select(b => b.HOut).FirstOrDefault().ToString() == "" ? "0" : round.ScoreCards.Where(a => a.Theme == "Fairway").Select(b => b.HOut).FirstOrDefault().ToString());

            round.InScore = int.Parse(round.ScoreCards.Where(a => a.Theme == "Score").Select(b => b.HIn).FirstOrDefault().ToString() == "" ? "0" : round.ScoreCards.Where(a => a.Theme == "Score").Select(b => b.HIn).FirstOrDefault().ToString());
            round.InPutt = int.Parse(round.ScoreCards.Where(a => a.Theme == "Putt").Select(b => b.HIn).FirstOrDefault().ToString() == "" ? "0" : round.ScoreCards.Where(a => a.Theme == "Putt").Select(b => b.HIn).FirstOrDefault().ToString());
            round.InGIR = double.Parse(round.ScoreCards.Where(a => a.Theme == "GIR").Select(b => b.HIn).FirstOrDefault().ToString() == "" ? "0" : round.ScoreCards.Where(a => a.Theme == "GIR").Select(b => b.HIn).FirstOrDefault().ToString());
            round.InFH = double.Parse(round.ScoreCards.Where(a => a.Theme == "Fairway").Select(b => b.HIn).FirstOrDefault().ToString() == "" ? "0" : round.ScoreCards.Where(a => a.Theme == "Fairway").Select(b => b.HIn).FirstOrDefault().ToString());

            // todo: do we need this?
            round.ScoreCards = listScoreCards;

            ViewData["scoreNumbers"] = scoreNumbers;
            ViewData["puttNumbers"] = puttNumbers;
            ViewData["fairwayNumbers"] = fairwayNumbers;
            ViewData["girNumbers"] = girNumbers;
            ViewData["pitchNumbers"] = pitchNumbers;
            ViewData["chipNumbers"] = chipNumbers;
            ViewData["gsBunkerNumbers"] = gsBunkerNumbers;
            ViewData["penaltyNumbers"] = penaltyNumbers;

            db.SaveChanges();

            return View(round);
        }

        // POST: Field/FieldEnterScoreHole
        // Step 2a
        [HttpPost]
        //public ActionResult FieldEnterScoreHole(string holeDistance) //Round round
        public ActionResult FieldEnterScoreHole(Round round)
        {
            // when FINISH button clicked
            // todo: need to pass score sum etc

            // we need to fill some columns of round for summary of the round
            round.Score = int.Parse(round.ScoreCards.Where(a => a.Theme == "Score").Select(b => b.HTotal).FirstOrDefault().ToString() == "" ? "0" : round.ScoreCards.Where(a => a.Theme == "Score").Select(b => b.HTotal).FirstOrDefault().ToString());
            round.Putt = int.Parse(round.ScoreCards.Where(a => a.Theme == "Putt").Select(b => b.HTotal).FirstOrDefault().ToString() == "" ? "0" : round.ScoreCards.Where(a => a.Theme == "Putt").Select(b => b.HTotal).FirstOrDefault().ToString());
            round.GIR = double.Parse(round.ScoreCards.Where(a => a.Theme == "GIR").Select(b => b.HTotal).FirstOrDefault().ToString() == "" ? "0" : round.ScoreCards.Where(a => a.Theme == "GIR").Select(b => b.HTotal).FirstOrDefault().ToString());
            round.FH = double.Parse(round.ScoreCards.Where(a => a.Theme == "Fairway").Select(b => b.HTotal).FirstOrDefault().ToString() == "" ? "0" : round.ScoreCards.Where(a => a.Theme == "Fairway").Select(b => b.HTotal).FirstOrDefault().ToString());

            round.OutScore = int.Parse(round.ScoreCards.Where(a => a.Theme == "Score").Select(b => b.HOut).FirstOrDefault().ToString() == "" ? "0" : round.ScoreCards.Where(a => a.Theme == "Score").Select(b => b.HOut).FirstOrDefault().ToString());
            round.OutPutt = int.Parse(round.ScoreCards.Where(a => a.Theme == "Putt").Select(b => b.HOut).FirstOrDefault().ToString() == "" ? "0" : round.ScoreCards.Where(a => a.Theme == "Putt").Select(b => b.HOut).FirstOrDefault().ToString());
            round.OutGIR = double.Parse(round.ScoreCards.Where(a => a.Theme == "GIR").Select(b => b.HOut).FirstOrDefault().ToString() == "" ? "0" : round.ScoreCards.Where(a => a.Theme == "GIR").Select(b => b.HOut).FirstOrDefault().ToString());
            round.OutFH = double.Parse(round.ScoreCards.Where(a => a.Theme == "Fairway").Select(b => b.HOut).FirstOrDefault().ToString() == "" ? "0" : round.ScoreCards.Where(a => a.Theme == "Fairway").Select(b => b.HOut).FirstOrDefault().ToString());

            round.InScore = int.Parse(round.ScoreCards.Where(a => a.Theme == "Score").Select(b => b.HIn).FirstOrDefault().ToString() == "" ? "0" : round.ScoreCards.Where(a => a.Theme == "Score").Select(b => b.HIn).FirstOrDefault().ToString());
            round.InPutt = int.Parse(round.ScoreCards.Where(a => a.Theme == "Putt").Select(b => b.HIn).FirstOrDefault().ToString() == "" ? "0" : round.ScoreCards.Where(a => a.Theme == "Putt").Select(b => b.HIn).FirstOrDefault().ToString());
            round.InGIR = double.Parse(round.ScoreCards.Where(a => a.Theme == "GIR").Select(b => b.HIn).FirstOrDefault().ToString() == "" ? "0" : round.ScoreCards.Where(a => a.Theme == "GIR").Select(b => b.HIn).FirstOrDefault().ToString());
            round.InFH = double.Parse(round.ScoreCards.Where(a => a.Theme == "Fairway").Select(b => b.HIn).FirstOrDefault().ToString() == "" ? "0" : round.ScoreCards.Where(a => a.Theme == "Fairway").Select(b => b.HIn).FirstOrDefault().ToString());

            if (ModelState.IsValid)
            {
                // update Rounds set [values...] where RID = round.RID
                Round currentRound = db.Rounds.FirstOrDefault(r => r.RID == round.RID);
                if (currentRound != null)
                {
                    db.Entry(currentRound).CurrentValues.SetValues(round);
                }
            }

            // insert into ScoreCards values ...
            foreach (ScoreCard score in round.ScoreCards)
            {
                //score.CreateDate = DateTime.Now;  // these two values are missing... why
                score.UpdateDate = DateTime.Now;
                score.FRID = round.RID;
                db.ScoreCards.Add(score);
            }

            db.Rounds.AddOrUpdate(round);

            //db.SaveChanges();
            return RedirectToAction("Index", "Round");
        }


        // a: source b: destination (copy properties in a to b)
        public void CopyPropertiesH(object a, object b)
        {
            Type typeB = b.GetType();

            foreach (PropertyInfo property in a.GetType().GetProperties())
            {
                if (!property.CanRead || (property.GetIndexParameters().Length > 0))
                    continue;

                PropertyInfo other = typeB.GetProperty(property.Name);
                if ((other != null) && (other.CanWrite) && other.Name.StartsWith("H") && Char.IsNumber(other.Name[1]))
                {
                    other.SetValue(b, property.GetValue(a, null), null);
                }
            }
        }
    }

    public class HoleScore
    {
        public int holeNo { get; set; }
        public int handi { get; set; }
        public int distance { get; set; }
        public int score { get; set; }
        public int altScore { get; set; }
        public int putt { get; set; }
        public int fairway { get; set; }
        public int gir { get; set; }
        public int pitch { get; set; }
        public int chip { get; set; }
        public int gsBunker { get; set; }
        public int penalty { get; set; }
    }

}
