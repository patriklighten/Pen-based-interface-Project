using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Ink;
using System.IO;// used for file stream 
using System.Runtime.Serialization.Formatters.Binary; // binary serialize
using System.Linq;
using System.Collections;
using System.Windows.Input;
using System.Runtime.InteropServices;
  
namespace WindowsFormsApplication1
{

    public partial class savetable : Form
    {
        [DllImport("user32.dll")]
        static extern bool GetCursorPos(ref Point lpPoint);

        private InkOverlay m_InkOverlay;
        Microsoft.Ink.Strokes RawTemplate;
        Microsoft.Ink.Strokes savestrokes;
        Microsoft.Ink.Strokes loadstrokes; 
        List<Point>[] templatePoints;// template strokes's resampled points 
        List<Point>[] UnistrokePt;// Strokes Points that being recognize;
        List<Point>[] Chair;
        int templateStrCounts;// count of the template's stroke, 0~19 for 0,1,2,3,4,5,6,7,8,9,+(10),-(11),*(12),t(13),A(14),n(15),s(16),c(17),i(18), and the square root (19) symbol
        List<Stroke>[] ExpressionStrks;// used to load the multiple strokes of an expression;
        int ExpressionCount = 0;// number of the symbols in the espression 
        List<Point>[] templatePoints0;
        List<Point>[] templatePoints1;
        List<Point>[] templatePointsof1;
        List<Point>[] templatePointsof2;
        List<Point>[] templatePointsof3;
        List<Point>[] templatePointsof4;
        List<Point>[] templatePointsof5;
        List<Point>[] templatePointsof6;
        List<Point>[] templatePointsof7;
        List<Point>[] templatePointsof8;
        List<Point>[] templatePointsof9;
        List<Point>[] templatePointsofPlus;
        List<Point>[] templatePointsofMultiply;
        List<Point>[] templatePointsSqrt;
        List<Point>[][] templateGroup = new List<Point>[15][]; // 15 templates; 
        string[] SymbolIndx;
        int numoftemples = 5;// number of templates
        int saveFlag = 0;// indicate if the template has been save;
        // LoadDeserializePt(templatePointsof1, "A:\\Ndollar\\templatepointsof1.data");
         int loadflag = 0;
        
         //static extern uint GetTickCount();
        // define structure for each of the expression symbols
        public struct ExprUnit
        {

            public int index;// the correspoind to the index of the units in  List<Stroke>[] ExpressionStrks
            public int IsOpr;
            public int IsNumber;
            public string symbol;//'1','2','S'( represent sqirt)
            public Rectangle boundingbox;
            // initilizaton function 

            public ExprUnit(int ind, int isop, int isN, string symb, Rectangle bdbox)// char symbol could not represent character?
            {

                index = ind;
                IsOpr = isop;
                IsNumber = isN;
                symbol = symb;
                boundingbox = bdbox;
            }
        }
        List<int> orderofsymbol = new List<int>();// index sample accord to their printed order



        [Serializable]
        public struct FrontChair
        {
           // all the member need to be public defined if they are used as programing public interfaces
          public   List<Point> chairshaft;// intersect line of the chair seat and chair back; 
          public  List<Point> chairleg_1;// left leg's index =0, right one is 1;
          public List<Point> chairleg_2 ;
          public List<Point> chairback ;
          public List<Point> chairseat ;

          public FrontChair(List<Point> chsft, List<Point> chlg1, List<Point> chlg2, List<Point> chbk, List<Point> chst)
          {
              chairshaft = new List<Point>();
              chairshaft = chsft;
              chairleg_1 = new List<Point>();
              chairleg_1 = chlg1;
              chairleg_2 = new List<Point>();
              chairleg_2 = chlg2;
              chairback = new List<Point>();
              chairback = chbk;
              chairseat = new List<Point>();
              chairseat = chst;
          }
        }// front view chair 

        FrontChair publicfrntChair = new FrontChair();
        // side view chair :4 strokes 
        [Serializable]
        public struct SideViewChair
        {
           
          public  List<Point> chairleg_1;// left leg's index =0, right one is 1;
          public List<Point> chairleg_2 ;
          public List<Point> chairback ;
          public List<Point> chairseat ;

          public SideViewChair(List<Point> chlg1, List<Point> chlg2, List<Point> chbk, List<Point> chst)
          {
              
              chairleg_1 = new List<Point>();
              chairleg_1 = chlg1;
              chairleg_2 = new List<Point>();
              chairleg_2 = chlg2;
              chairback = new List<Point>();
              chairback = chbk;
              chairseat = new List<Point>();
              chairseat = chst;
          }
        }

        SideViewChair publicsideChair = new SideViewChair();

        // people model
        [Serializable]
        public struct peopleMd
        {

            public List<Point> leg_1;// left leg's index =0, right one is 1;
            public List<Point> leg_2;
            public List<Point> hand_1;// left leg's index =0, right one is 1;
            public List<Point> hand_2;
            public List<Point> head;
            public List<Point> back;

            public peopleMd(List<Point> lg1, List<Point> lg2, List<Point> bk, List<Point> hd1, List<Point> hd2, List<Point> hd)
            {

                leg_1 = new List<Point>();
                leg_1 = lg1;
                leg_2 = new List<Point>();
                leg_2 = lg2;
                back = new List<Point>();
                back = bk;
                hand_1 = new List<Point>();
                hand_1 = hd1;
                hand_2 = new List<Point>();
                hand_2 = hd2;
                head = new List<Point>();
                head = hd;
            }
        } 
        // used to load the public plmd
        peopleMd pubicPlmd = new peopleMd();

        [Serializable]
        public struct table
        {

            public List<Point> chairleg_1;// left leg's index =0, right one is 1;
            public List<Point> chairleg_2;
            public List<Point> surface;
          

            public table(List<Point> chlg1, List<Point> chlg2, List<Point> surface1)
            {

                chairleg_1 = new List<Point>();
                chairleg_1 = chlg1;
                chairleg_2 = new List<Point>();
                chairleg_2 = chlg2;
                surface = new List<Point>();
                surface = surface1;
                
            }
        }
        table PublicTable = new table();

        [Serializable]
        public struct scene
        {
            public List<table> tables; 
            public List<FrontChair> frontchairs; 
            public List<SideViewChair> sideViewChairs;
            public List<peopleMd> PeopleGroup;


            // initialization 
            public scene(  List<table> TBs, List<FrontChair>FRChs,List<SideViewChair> SVDChs, List<peopleMd> PLGrp)
            {

               tables= new List<table>();
               tables = TBs;
               frontchairs = new List<FrontChair>();
                frontchairs=FRChs;
               sideViewChairs = new List<SideViewChair>();
                 sideViewChairs=SVDChs;
                 PeopleGroup = new List<peopleMd>();
                 PeopleGroup = PLGrp;
                
            }

        }

        scene publicScene = new scene();

        [Serializable]
         public struct spatialInformation
        {
            int numberofRtPl;
            int numberofLtPl;
            int numberofTable;
            int numberLtChair;
            int numberRtChair; 
            int numberCompt;
          
            public spatialInformation(int NRtPl, int NLtPl, int Ntable, int NLtChair, int NRtChair, int Ncompt)
            {
                numberCompt=Ncompt;
                numberofLtPl=NLtPl;
                numberofRtPl=NRtPl;
                numberLtChair=NLtChair;
                numberRtChair=NRtChair;
                numberofTable=Ntable;
            }
        }

        [Serializable]
        public struct SdvchairInfo
        {
           public  int viewside; // left view 0, right view 1
         public  Point centerLocation;// location of center point 
     //    public int tableIndex;// the table the chair is near to 
         
            public SdvchairInfo( int view, Point center)
        {
            viewside = view;
            centerLocation = center;
            //tableIndex = tableindex;// -1 means no table 
        }

        }


        [Serializable]
        public struct PeopleInfo
        {
            public int viewside;//left view 0, right view 2, front view 3;
            public int sitOrstand;// 0 sit, 1 stand 
            public int sitingchair_index; // 
            public int typeofChair;// 0 side view, 1 front view 
            public Point CentLocation;

            public PeopleInfo(int view, int sitstand, int chairindex, int typechair, Point center)
            {
                viewside = view;
                sitOrstand = sitstand;
                sitingchair_index = chairindex;
                typeofChair = typechair;
                CentLocation = center;

            }
            
            }
        [Serializable]
        public struct tableInfo
        {
          public Point Centlocation;
          
          public tableInfo(Point center)
          {
              Centlocation = center;
          }
        }

         [Serializable]

        public struct frontChInfo
        {
            public int view;// left 0,right 1, front 2
            public Point centerLocation;
         // public   int tableIndex;// -1 means no nearby table 

            public frontChInfo(int viewside, Point Center)
            {
                view = viewside;
                centerLocation = Center;
                //tableIndex = tableindex;

            }

        }

        [Serializable]
        public struct SceneSturcture
        {
          public   List<SdvchairInfo> SdvChairsInfo;
          public   List<PeopleInfo> PeopleGrpInfo;
          public   List<frontChInfo> FrntChairsInfo;
          public  List<tableInfo> tablesInfo;
        }

        SceneSturcture publicSceneStructure = new SceneSturcture();  // the object's arrangement information 

       
      // complex object representation in point set and frame 
        [Serializable]
        public struct ComplexHumanModel
        {
          public   List<Point> points;
          public  peopleMd frame;

            public ComplexHumanModel(List<Point> pts, peopleMd structure)
            {
                points = new List<Point>();
                points = pts;
                frame = new peopleMd();
                frame = structure;
            }

        }

        List<Point> public_complex_ObjectPoints = new List<Point>();

        ComplexHumanModel public_complex_humanModel = new ComplexHumanModel();

        [Serializable]
        public struct complextFrntChair
        {
          public   List<Point> points;
          public   FrontChair frame;
          public complextFrntChair(List<Point>pts, FrontChair structure)
          {
              points = new List<Point>();
              points = pts;
              frame = new FrontChair();
              frame = structure;
          }

            
        }

        complextFrntChair public_complex_FrontChair = new complextFrntChair();

        // complex side view chair 
        [Serializable]
        public struct complextSdvChair
        {
            public List<Point> points;
            public SideViewChair frame;
            public complextSdvChair(List<Point> pts, SideViewChair structure)
            {
                points = new List<Point>();
                points = pts;
                frame = new SideViewChair();
                frame = structure;
                
            }

        }

        complextSdvChair public_complex_SdvChair = new complextSdvChair();


        // complex table

        [Serializable]
        public struct complexTable
        {
            public List<Point> points;
            public table frame;
            public complexTable(List<Point> pts, table structure)
            {
                points = new List<Point>();
                points = pts;
                frame = new table();
                frame = structure;

            }

        }
        complexTable public_complex_Table = new complexTable();


       // spatialInformation publicobjectInformation;
        // a set of template for different status of object
        SideViewChair public_LeftSdVCh= new SideViewChair();
        SideViewChair public_RightSdVCh= new SideViewChair();
        FrontChair public_LeftFrntChair= new FrontChair();
        FrontChair public_RightFrntChair= new FrontChair();
        peopleMd public_Stand_Plmd= new peopleMd();
        //peopleMd public_Stand_LeftPLmd= new peopleMd();
        peopleMd public_Sit_RightPlmd= new peopleMd();
        peopleMd public_Sit_LeftPlmd= new peopleMd();
        peopleMd public_Sit_FrntPlmd= new peopleMd();
        peopleMd public_Stand_LeftPlmd= new peopleMd();
 


        public savetable()
        {
            InitializeComponent();

            m_InkOverlay = new InkOverlay(Handle);
            m_InkOverlay.Enabled = true;

            // Initialize the Expression Loader, assume the number of the symbols in Expression no more than 20;

            // load object template    
            load_object_template();

            // initialize  scene structure 
            Initialize_SceneStructure();

            Initialize_SceneStructure1();
            //LoadTemplate("A:\\Pen_based\\templates\\Atemplate.isf", RawTemplate);

            m_InkOverlay.Enabled = false;
            m_InkOverlay.Ink = new Ink();
            m_InkOverlay.Enabled = true;
            // read the bytes from the file
            FileStream fs = new FileStream("A:\\Pen_based\\templates\\9template.isf", FileMode.Open, FileAccess.ReadWrite);
            byte[] isf = new byte[fs.Length];
            fs.Read(isf, 0, (int)fs.Length);
            // and load the Ink object
            m_InkOverlay.Ink.Load(isf);
            Ink templateInk = m_InkOverlay.Ink.Clone();// Use the Clone to copy the ink, otherwise the only copy the PT of the ink, which would makes the template ink point to the dynamic ink

            //   m_InkOverlay.Ink.DeleteStrokes(); // the template strokes will not show if delete the ink that contain it

            RawTemplate = templateInk.Strokes;
            //  m_InkOverlay.Ink.DeleteStrokes();
            // load the points in each stroke of the strokes 
            templatePoints = new List<Point>[RawTemplate.Count];
            templateStrCounts = templatePoints.Count();
            for (int i = 0; i < RawTemplate.Count; i++)
            {
                templatePoints[i] = new List<Point>();
            }
            // above is template points 

            //Note add loadtemplate code here, so that load thread is not conflict with the following Stroke add and overlay Painted event 

            // selection strokes which will be as an input for recognition
            //  load a set of templatepoints for recognition
            templatePointsof1 = LoadDeserializePt(templatePointsof1, "templatepointsof1.data");// you need to return the value, because the function could not change the parameter's value, unless it is constant reference (&)
            templatePointsof2 = LoadDeserializePt(templatePointsof2, "templatepointsof2.data");
            templatePointsof3 = LoadDeserializePt(templatePointsof3, "templatepointsof3.data");
            templatePointsof4 = LoadDeserializePt(templatePointsof4, "templatepointsof4.data");
            templatePointsof5 = LoadDeserializePt(templatePointsof5, "templatepointsof5.data");
            templatePointsof6 = LoadDeserializePt(templatePointsof6, "templatepointsof6.data");
            templatePointsof7 = LoadDeserializePt(templatePointsof7, "templatepointsof7.data");
            templatePointsof8 = LoadDeserializePt(templatePointsof8, "templatepointsof8.data");
            templatePointsof9 = LoadDeserializePt(templatePointsof9, "templateof9.data");
            templatePointsofPlus = LoadDeserializePt(templatePointsofPlus, "templatepointsofPlus.data");
            templatePointsofMultiply = LoadDeserializePt(templatePointsofMultiply, "templateofMultiply.data");
            templatePointsSqrt = LoadDeserializePt(templatePointsSqrt, "templateofSqrt.data");
            // Create the event handler to respond to a StrokeAdded event.
            m_InkOverlay.Stroke += new InkCollectorStrokeEventHandler(InkStrokeAdded);
            // Hook up to the InkOverlay's event handlers.
            m_InkOverlay.Painted += new InkOverlayPaintedEventHandler(InkPainted);
            templateGroup[0] = templatePointsof1;
            templateGroup[1] = templatePointsof2;
            templateGroup[2] = templatePointsof3;
            templateGroup[3] = templatePointsof4;
            templateGroup[4] = templatePointsof5;
            templateGroup[5] = templatePointsof6;
            templateGroup[6] = templatePointsof7;
            templateGroup[7] = templatePointsof8;
            templateGroup[8] = templatePointsof9;
            templateGroup[9] = templatePointsofPlus;
            templateGroup[10] = templatePointsofMultiply;
            templateGroup[11] = templatePointsSqrt;
            // symbol index
            SymbolIndx = new string[] { "1", "2", "3", "4", "5", "6", "7", "8", "9", "+", "*", "S" };// temporarily 12 symbols

            //SymbolIndx={'1','2','3','4','5','6','7','8','9','+','*','s'};// S represent Sqrt;
            // print the scene of " peoplemd sit on the chair" onto the from 
         

        }//end of form1


        private void InkPainted(object sender, PaintEventArgs e)
        {
            // Call the helper function that redraws the form.
            Renderer renderer = new Renderer();
            DrawStrokeIds(renderer, e.Graphics, Font, m_InkOverlay.Ink.Strokes, m_InkOverlay.Selection);
        }

        private void InkStrokeAdded(object sender, InkCollectorStrokeEventArgs e)
        {
            // show the points information in the textBox1  

           // this.Invalidate();// 只有把这些 当前的窗口invalidate, 或者 refesth(); 这时候窗口的坐标才可以用 renderer.InkSpaceToPixel转换为 pixels 坐标系
        }

        // Draw the Stroke IDs for a Strokes collection.
        public void DrawStrokeIds(
           Renderer renderer, Graphics g, Font font, Strokes strokes, Strokes selectionstrks)
        {


            string text = "";
            textBox1.Clear();
            int N = 60;
            int DetectCorners = 0;
            int DetectIntersect = 0;
            List<int> cornerlist = new List<int>();
            List<int> intersectionPt = new List<int>();

            List<Point> Resamplelist = new List<Point>();
            List<Point> selectionlist = new List<Point>();




            //load templates
          
            
            // test, don't include the function here, otherwise the function would call every time you draw on the form, TemplateStrokToPt(RawTemplate, templatePoints, renderer, g);

            // process the selection strokes
            // translate the selection strokes to a unit points list;

            if (m_InkOverlay.EditingMode != InkOverlayEditingMode.Delete && loadflag == 1)
            {
                loadflag = 0;
                for (int i = 1; i < loadstrokes.Count; i++)
                {


                    List<Point> points1 = ResamPlePt(renderer, g, loadstrokes[i].GetPoints(), 60);
                   // List<Point> points2 = ResamPlePt(renderer, g, loadstrokes[i - 1].GetPoints(), 60);
                }

            }

            if (m_InkOverlay.EditingMode != InkOverlayEditingMode.Delete && m_InkOverlay.EditingMode == InkOverlayEditingMode.Select)
            {
                   // assume that the front chair is comprise of 5 strokes, namely, back, middle shaft, seat, leftleg(leg1), rightleg(leg2)
                if (selectionstrks.Count==5)
                {
                    
                    int strkcount=selectionstrks.Count;
                    Stroke[] frntchairstks = new Stroke[selectionstrks.Count];
                    for (int i = 0; i < selectionstrks.Count; i++)
                    {
                        frntchairstks[i] = selectionstrks[i];
                    }

                    // Array.Sort(ExpressionU, (x, y) => (x.boundingbox.Left).CompareTo(y.boundingbox.Left));//the order of (x.boundingbox.Left).CompareTo(y.boundingbox.Left)
                    Array.Sort(frntchairstks, (x, y) => (x.GetBoundingBox().Top + x.GetBoundingBox().Bottom).CompareTo(y.GetBoundingBox().Top + y.GetBoundingBox().Bottom));// sort according to the bounding box's center point's y from large to small 
                    Stroke chairleg1, chairleg2, chairseat, chairback, chairshaft;
                    if (frntchairstks[strkcount - 2].GetBoundingBox().Left < frntchairstks[strkcount - 1].GetBoundingBox().Left)
                    {
                        chairleg1 = frntchairstks[strkcount - 2];
                        chairleg2 = frntchairstks[strkcount - 1];
                    }
                    else
                    {
                        chairleg1 = frntchairstks[strkcount - 1];
                        chairleg2 = frntchairstks[strkcount - 2];
                    }

                    chairback = frntchairstks[0];
                    chairseat = frntchairstks[2];
                    chairshaft = frntchairstks[1];
                    List<Point> chairbackPt=ResamPlePt(renderer, g, chairback.GetPoints(), chairback.GetPoints().Count() / 2);
                    List<Point> chairseatPt = ResamPlePt(renderer, g, chairseat.GetPoints(),chairseat.GetPoints().Count() / 2);
                    List<Point> chairshaftPt = ResamPlePt(renderer, g, chairshaft.GetPoints(), chairshaft.GetPoints().Count() / 2);
                    List<Point> chairleg1Pt = ResamPlePt(renderer, g, chairleg1.GetPoints(), chairleg1.GetPoints().Count() / 2);
                    List<Point> chairleg2Pt = ResamPlePt(renderer, g, chairleg2.GetPoints(), chairleg2.GetPoints().Count() / 2);
                    FrontChair frntchair = new FrontChair(chairshaftPt, chairleg1Pt, chairleg2Pt, chairbackPt, chairseatPt);

                    // save to binary file
                    using (FileStream fs = new FileStream("ChairStrk1.data", FileMode.Create, FileAccess.Write))
                    {


                        BinaryFormatter b = new BinaryFormatter();
                        b.Serialize(fs, frntchair);
                        fs.Close();

                    }

                    // load data structure from file 
                    FrontChair frntchair1 = new FrontChair();
                    using (FileStream fs1 = new FileStream("ChairStrk1.data", FileMode.Open))
                    {
                        BinaryFormatter formatter = new BinaryFormatter();
                        frntchair1 = (FrontChair)formatter.Deserialize(fs1);
                    }


                    for (int i1=0;i1<frntchair1.chairleg_1.Count();i1++)
                        g.DrawRectangle(Pens.Red, frntchair1.chairleg_1[i1].X, frntchair1.chairleg_1[i1].Y, 2, 2);
                    for (int i1 = 0; i1 < frntchair1.chairleg_2.Count(); i1++)
                        g.DrawRectangle(Pens.Green, frntchair1.chairleg_2[i1].X, frntchair1.chairleg_2[i1].Y, 2, 2);
                    for (int i1 = 0; i1 < frntchair1.chairback.Count(); i1++)
                        g.DrawRectangle(Pens.Blue, frntchair1.chairback[i1].X, frntchair1.chairback[i1].Y, 2, 2);
                    for (int i1 = 0; i1 < frntchair1.chairseat.Count(); i1++)
                        g.DrawRectangle(Pens.Yellow, frntchair1.chairseat[i1].X, frntchair1.chairseat[i1].Y, 2, 2);
                    for (int i1 = 0; i1 < frntchair1.chairshaft.Count(); i1++)
                        g.DrawRectangle(Pens.YellowGreen, frntchair1.chairshaft[i1].X, frntchair1.chairshaft[i1].Y, 2, 2);
                    Point referPt1 = CentroidPoint(frntchair1.chairshaft.ToArray());// center point of the cross chairshaft 
                    g.DrawRectangle(Pens.Red, referPt1.X, referPt1.Y, 2, 2);

                    publicfrntChair = frntchair1;
                }

                // store the side view chair 
                if (selectionstrks.Count == 4)
                {

                    int strkcount = selectionstrks.Count;
                    Stroke[] frntchairstks = new Stroke[selectionstrks.Count];
                    for (int i = 0; i < selectionstrks.Count; i++)
                    {
                        frntchairstks[i] = selectionstrks[i];
                    }

                    // Array.Sort(ExpressionU, (x, y) => (x.boundingbox.Left).CompareTo(y.boundingbox.Left));//the order of (x.boundingbox.Left).CompareTo(y.boundingbox.Left)
                    Array.Sort(frntchairstks, (x, y) => (x.GetBoundingBox().Top + x.GetBoundingBox().Bottom).CompareTo(y.GetBoundingBox().Top + y.GetBoundingBox().Bottom));// sort according to the bounding box's center point's y from large to small 
                    Stroke chairleg1, chairleg2, chairseat, chairback, chairshaft;
                    if (frntchairstks[strkcount - 2].GetBoundingBox().Left < frntchairstks[strkcount - 1].GetBoundingBox().Left)
                    {
                        chairleg1 = frntchairstks[strkcount - 2];
                        chairleg2 = frntchairstks[strkcount - 1];
                    }
                    else
                    {
                        chairleg1 = frntchairstks[strkcount - 1];
                        chairleg2 = frntchairstks[strkcount - 2];
                    }

                    chairback = frntchairstks[0];
                    chairseat = frntchairstks[1];
                    
                    List<Point> chairbackPt = ResamPlePt(renderer, g, chairback.GetPoints(), chairback.GetPoints().Count() / 2);
                    List<Point> chairseatPt = ResamPlePt(renderer, g, chairseat.GetPoints(), chairseat.GetPoints().Count() / 2);
                    
                    List<Point> chairleg1Pt = ResamPlePt(renderer, g, chairleg1.GetPoints(), chairleg1.GetPoints().Count() / 2);
                    List<Point> chairleg2Pt = ResamPlePt(renderer, g, chairleg2.GetPoints(), chairleg2.GetPoints().Count() / 2);
                    SideViewChair sidechair = new SideViewChair(chairleg1Pt, chairleg2Pt, chairbackPt, chairseatPt);

                    // save to binary file
                    using (FileStream fs = new FileStream("sidechair1.data", FileMode.Create, FileAccess.Write))
                    {


                        BinaryFormatter b = new BinaryFormatter();
                        b.Serialize(fs, sidechair);
                        fs.Close();

                    }

                    // load data structure from file 
                    SideViewChair sidechair1 = new SideViewChair();
                    using (FileStream fs1 = new FileStream("sidechair1.data", FileMode.Open))
                    {
                        BinaryFormatter formatter = new BinaryFormatter();
                        sidechair1 = (SideViewChair)formatter.Deserialize(fs1);
                    }

                    publicsideChair = sidechair;
               
                    for (int i1 = 0; i1 < sidechair1.chairleg_1.Count(); i1++)
                        g.DrawRectangle(Pens.Red, sidechair1.chairleg_1[i1].X, sidechair1.chairleg_1[i1].Y, 2, 2);
                    for (int i1 = 0; i1 < sidechair1.chairleg_2.Count(); i1++)
                        g.DrawRectangle(Pens.Green, sidechair1.chairleg_2[i1].X, sidechair1.chairleg_2[i1].Y, 2, 2);
                    for (int i1 = 0; i1 < sidechair1.chairback.Count(); i1++)
                        g.DrawRectangle(Pens.Blue, sidechair1.chairback[i1].X, sidechair1.chairback[i1].Y, 2, 2);
                    for (int i1 = 0; i1 < sidechair1.chairseat.Count(); i1++)
                        g.DrawRectangle(Pens.Yellow, sidechair1.chairseat[i1].X, sidechair1.chairseat[i1].Y, 2, 2);
                 
                }

                if (selectionstrks.Count == 6)
                {
                    peopleMd plmd = CreatPlMd(selectionstrks, renderer, g);

                    pubicPlmd = plmd;

                    for (int i1 = 0; i1 < plmd.leg_1.Count(); i1++)
                        g.DrawRectangle(Pens.Red, plmd.leg_1[i1].X, plmd.leg_1[i1].Y, 2, 2);
                    for (int i1 = 0; i1 < plmd.leg_2.Count(); i1++)
                        g.DrawRectangle(Pens.Yellow, plmd.leg_2[i1].X, plmd.leg_2[i1].Y, 2, 2);
                    for (int i1 = 0; i1 < plmd.head.Count(); i1++)
                        g.DrawRectangle(Pens.Blue, plmd.head[i1].X, plmd.head[i1].Y, 2, 2);
                    for (int i1 = 0; i1 < plmd.back.Count(); i1++)
                        g.DrawRectangle(Pens.Yellow, plmd.back[i1].X, plmd.back[i1].Y, 2, 2);
                    for (int i1 = 0; i1 < plmd.hand_1.Count(); i1++)
                        g.DrawRectangle(Pens.Green, plmd.hand_1[i1].X, plmd.hand_1[i1].Y, 2, 2);
                    for (int i1 = 0; i1 < plmd.hand_2.Count(); i1++)
                        g.DrawRectangle(Pens.Green, plmd.hand_2[i1].X, plmd.hand_2[i1].Y, 2, 2);
                    Point[] backpt = plmd.back.ToArray();
                    Point referPt2 = new Point();
                    Array.Sort(backpt, (x, y) => (x.Y).CompareTo(y.Y));// sort in decent order;
                    referPt2 = backpt[(backpt.Count() * 3 / 4)];
                    g.DrawRectangle(Pens.Green, referPt2.X, referPt2.Y, 3, 3);
                   
                }
                
                // recognizetable frame 
                if (selectionstrks.Count == 3)
                {
                    Stroke[] tablestrk = new Stroke[m_InkOverlay.Selection.Count];
                    int counts = m_InkOverlay.Selection.Count;
                    for (int i = 0; i < m_InkOverlay.Selection.Count; i++)
                    {
                        tablestrk[i] = m_InkOverlay.Selection[i];

                    }
                    Array.Sort(tablestrk, (x, y) => (x.GetBoundingBox().Top + x.GetBoundingBox().Bottom).CompareTo(y.GetBoundingBox().Top + y.GetBoundingBox().Bottom));// sort according to the bounding box's center point's y from large to small 
                    Stroke leg1, leg2, surface;
                    if (tablestrk[counts - 2].GetBoundingBox().Left < tablestrk[counts - 1].GetBoundingBox().Left)
                    {
                        leg1 = tablestrk[counts - 2];
                        leg2 = tablestrk[counts - 1];
                    }
                    else
                    {
                        leg1 = tablestrk[counts - 1];
                        leg2 = tablestrk[counts - 2];
                    }
                    surface = tablestrk[0];
                    List<Point> leg1Pt = ResamPlePt(renderer, g, leg1.GetPoints(), leg1.GetPoints().Count() / 2);
                    List<Point> leg2Pt = ResamPlePt(renderer, g, leg2.GetPoints(), leg2.GetPoints().Count() / 2);

                    List<Point> surfacePt = ResamPlePt(renderer, g, surface.GetPoints(), surface.GetPoints().Count() / 2);
                    PublicTable = new table(leg1Pt, leg2Pt, surfacePt);

                    for (int i1 = 0; i1 < PublicTable.chairleg_1.Count(); i1++)
                        g.DrawRectangle(Pens.Red, PublicTable.chairleg_1[i1].X, PublicTable.chairleg_1[i1].Y, 2, 2);
                    for (int i1 = 0; i1 < PublicTable.chairleg_2.Count(); i1++)
                        g.DrawRectangle(Pens.Green, PublicTable.chairleg_2[i1].X, PublicTable.chairleg_2[i1].Y, 2, 2);
                    for (int i1 = 0; i1 < PublicTable.surface.Count(); i1++)
                        g.DrawRectangle(Pens.Blue, PublicTable.surface[i1].X, PublicTable.surface[i1].Y, 2, 2);

                    using (FileStream fs = new FileStream(" PublicTable.data", FileMode.Create, FileAccess.Write))
                    {


                        BinaryFormatter b = new BinaryFormatter();
                        b.Serialize(fs, PublicTable);// save the unistrokePt that is the resampled and reprocessing strokes points
                        fs.Close();

                    }


                }

                //  List< Point>[] chair1=  LoadDeserializePt(Chair, "chair.data");
                //   chair1[0].
                //  Array.Sort(ExpressionU, (x, y) => (x.boundingbox.Left).CompareTo(y.boundingbox.Left));
              ExpressionCount = 0;// reset the expressionCount 
                // gether related strokes: split the the inkstrokes of an expression into severl sets, each set represent a symbol unit;
                int flagindex = 0;

           /*     if (selectionstrks.Count > 0)
                {
                    ExpressionCount = 0;// reset to zero
                    // refresh the expressionstrk every select event, this is very important 
                    ExpressionStrks = new List<Stroke>[20];
                    // refresh the ExpressionU


                    for (int i = 0; i < 20; i++)// suppose there are at most 20 symbols in a expression
                    {
                        ExpressionStrks[i] = new List<Stroke>();

                    }
                    ExpressionStrks[0].Add(selectionstrks[0]);

                    for (int i = 1; i < selectionstrks.Count; i++)
                    {

                        int intersectFlag = 0;
                        List<Point> points1 = ResamPlePt(renderer, g, selectionstrks[i].GetPoints(), 60);
                        List<Point> points2 = ResamPlePt(renderer, g, selectionstrks[i - 1].GetPoints(), 60);

                        foreach (Point point1 in points1)
                        {
                            foreach (Point point2 in points2)
                                if (distance(point1.X, point1.Y, point2.X, point2.Y) < 5)
                                    intersectFlag = 1;

                        }
                        //  if (selectionstrks[i].GetBoundingBox().IntersectsWith(selectionstrks[i - 1].GetBoundingBox()))//if two strokes bounding box intersect with each other, then they belong to a symbol
                        if (intersectFlag == 1)// if  ink strokes are intersect with each other they are belong to same symbol unit
                        {

                            ExpressionStrks[ExpressionCount].Add(selectionstrks[i]);
                            intersectFlag = 0;
                        }
                        else
                        {
                            ExpressionCount = ExpressionCount + 1;
                            ExpressionStrks[ExpressionCount].Add(selectionstrks[i]);
                        }
                    }// complete gether strokes that belong to a same symbol unit

                    // load the imformation of each the express units to the expression struct 

                    // the ExprUnit should instantialized here, because the ExpressionCount is available here
                    ExprUnit[] ExpressionU = new ExprUnit[ExpressionCount + 1];// public ExprUnit(int ind, int isop, int isN, char symb, Rectangle bdbox)
                    // note that the ExpressionCount+1 is the total number of the unistrokes
                    for (int u = 0; u < ExpressionCount + 1; u++)
                    {
                        Strokes strks;
                        Stroke str = ExpressionStrks[0][0];
                        //   strks = (Strokes) str;
                        // get bounding box information 
                        Rectangle boundbox = new Rectangle();
                        for (int i1 = 0; i1 < ExpressionStrks[u].Count; i1++)
                        {

                            if (i1 == 0)
                                boundbox = ExpressionStrks[u][i1].GetBoundingBox();

                                    // union several bounding box    
                            else
                            {
                                boundbox = Rectangle.Union(boundbox, ExpressionStrks[u][i1].GetBoundingBox());
                            }
                        }
                        List<Stroke> strklist = new List<Stroke>();
                        for (int j = 0; j < ExpressionStrks[u].Count; j++)
                        {
                            strklist.Add(ExpressionStrks[u][j]);
                        }
                        //recognizeSymb can be used to recognize a element strokes
                        flagindex = recognizeSymb(strklist, boundbox, g, N, renderer);

                        //textBox1.Clear();
                        //textBox1.Text = flagindex.ToString();
                        // recognizeSymb(selectionstrks,selectionstrks.GetBoundingBox() , g, N, renderer);
                        if (flagindex < 9)
                            ExpressionU[u] = new ExprUnit(u, 0, 1, SymbolIndx[flagindex], boundbox);
                        if (flagindex >= 9 && flagindex < 12)
                            ExpressionU[u] = new ExprUnit(u, 1, 0, SymbolIndx[flagindex], boundbox);
                    }

                    //sort the ExpressionU by its element's left border of the element's boundingbox
                    Array.Sort(ExpressionU, (x, y) => (x.boundingbox.Left).CompareTo(y.boundingbox.Left));//the order of (x.boundingbox.Left).CompareTo(y.boundingbox.Left)

                    //test


                    if (ExpressionCount >= 2)
                    {
                        //UnistrokePt = new List<Point>[ExpressionStrks[2].Count()];
                        for (int j = 0; j < ExpressionStrks[1].Count(); j++)
                        {
                            Point[] points = ExpressionStrks[1][j].GetPoints();
                            selectionlist = ResamPlePt(renderer, g, points, points.Count());
                            for (int r = 0; r < selectionlist.Count(); r++)
                                g.DrawRectangle(Pens.Red, selectionlist[r].X, selectionlist[r].Y, 2, 2);
                        }
                    }


                    string text1 = "";
                    for (int t = 0; t < ExpressionCount + 1; t++)
                        text1 += ExpressionU[t].symbol;

                    char[] inFix = text1.ToCharArray();
                    char[] postFix = new char[100];
                    string val = "";
                    string postfixstr = "";
                    postfixstr = inFix2PostFix(inFix, postFix);
                    //textBox1.Text = postfixstr;

                    char[] postfix1 = postfixstr.ToCharArray();
                    val = postFixEval(postfix1).ToString();
                    //val=(char) ('9'-'0');
                    //   int text2 = '9' - '0';
                    // because the last element of postfixstr is /0 so that the string or char that behind the /0 is invisible
                    string showstring = "";
                    for (int i = 0; i < postfixstr.Length - 1; i++)
                        showstring += postfixstr[i];
                    string text2 = showstring + "=" + val.ToString();// if we use the postfixstr+"="+val.ToString(), the text only show the postfixstr; because the last element of the postfix str is /0;
                    textBox1.Text = text2;
                }

*/
                // recognize one symbol only, a symbol contains at least one ink strokes ; this function is important to save function,because it process the selected strokes in to points 
                
            }// if it is selection mode

            // Iterate through every Stroke referenced by the collection.
            if (m_InkOverlay.EditingMode != InkOverlayEditingMode.Delete && m_InkOverlay.EditingMode != InkOverlayEditingMode.Select)
            {

                for (int r = 0; r < strokes.Count; r++) // don't draw the template 
                //foreach (Stroke s in strokes)
                {
                    Stroke s = strokes[r];

                    Point[] points = s.GetPoints();
                    Resamplelist = ResamPlePt(renderer, g, points, N);
                    Point[] resamplepoints = Resamplelist.ToArray();
                    double indictiveangle = IndicAngle(Resamplelist, Resamplelist.Count(), g);

                    // resamplepoints= RotateBy(resamplepoints, -indictiveangle);
                    Rectangle boundbox = strokes.GetBoundingBox();
                    resamplepoints = ScaleDimTo(resamplepoints, 960, 0.3, boundbox);
                    Point centpoint = CentroidPoint(resamplepoints);
                    Point originpoint = new Point();

                    originpoint.X = 10;
                    originpoint.Y = 10;
                    // translate to originpoint 
                    resamplepoints = translateToPt(resamplepoints, centpoint, originpoint);


                    // store the templates to  templatePoints
                    if (r < templateStrCounts)  // templateStrCounts[14] is for "A" symbol
                    {

                        foreach (Point pointr in resamplepoints)
                            templatePoints[r].Add(pointr);
                    }

                    //drawing points
                    for (int i = 0; i < resamplepoints.Count(); i++)
                        g.DrawRectangle(Pens.Blue, resamplepoints[i].X, resamplepoints[i].Y, 2, 2);
                    //for test // g.DrawRectangle(Pens.Blue, templatePoints[r][i].X, templatePoints[r][i].Y, 2, 2) 
                    // drawing strokes

                    // Make sure each Stroke has not been deleted.

                    if (!s.Deleted)
                    {

                        // Draw the Stroke's ID at its starting point.
                        string str = s.Id.ToString();
                        Point pt = s.GetPoint(0);
                     
                        //PointToScreen(position);
                       // textBox2.Text = "X:" + position.X + "_Y:" + position.Y;
                        renderer.InkSpaceToPixel(g, ref pt);// render the inkspace to pixel space 

                        g.DrawString(
                           str, font, Brushes.White, pt.X - 10, pt.Y - 10);
                        g.DrawString(
                           str, font, Brushes.White, pt.X + 1, pt.Y + 1);
                        g.DrawString(
                           str, font, Brushes.Black, pt.X, pt.Y);
                       
                        //textBox2.Text = "begining X:" + pt.X + "begining Y:" + pt.Y;

                        textBox2.Text = " X:" + pt.X + "_Y:" + pt.Y;



                    }

                }// end of   for( int r=0; r<strokes.Count;r++)
            }
            // serialize the points to a file
           
        }

        private int IsScribbleGesture(int firstpt, int secondpt, Point[] resampledpt)
        {
            double distan = 0;
            double distan1 = 0;// the arcs distance between two points
            double distan2 = 0;// the line distance between two points
            int flg = 0;
            if (firstpt >= 0 && firstpt < resampledpt.Count() && secondpt >= 0 && secondpt < resampledpt.Count())
            {
                for (int i = firstpt; i < secondpt; i++)
                {
                    distan = Math.Pow(resampledpt[i + 1].X - resampledpt[i].X, 2) + Math.Pow(resampledpt[i + 1].Y - resampledpt[i].Y, 2);
                    distan = Math.Sqrt(distan);
                    distan1 += distan;
                }
                distan2 = Math.Pow(resampledpt[secondpt].X - resampledpt[firstpt].X, 2) + Math.Pow(resampledpt[secondpt].Y - resampledpt[firstpt].Y, 2);
                distan2 = Math.Sqrt(distan2);
                if (distan1 != 0)// distant2 can be zero, when the two point intersect 
                {
                    // textBox1.Text += "(" + (distan2 / distan1) + "),"; // for test;
                    if (distan2 / distan1 < (1 / 3.6))
                        flg = 1;
                }

            }
            if (flg == 1)
            {
                textBox1.Text += "flg=" + flg + ",";// for test;
                return 1;

            }
            else
            {
                // textBox1.Text += "flg=" + flg + ",";// for test;
                return 0;
            }
        }
        private int isline(int firstpt, int secondpt, Point[] resampledpt)
        {
            double distan = 0;
            double distan1 = 0;// the arcs distance between two points
            double distan2 = 0;// the line distance between two points
            int flg = 0;
            if (firstpt >= 0 && firstpt < resampledpt.Count() && secondpt >= 0 && secondpt < resampledpt.Count())
            {
                for (int i = firstpt; i < secondpt; i++)
                {
                    distan = Math.Pow(resampledpt[i + 1].X - resampledpt[i].X, 2) + Math.Pow(resampledpt[i + 1].Y - resampledpt[i].Y, 2);
                    distan = Math.Sqrt(distan);
                    distan1 += distan;
                }
                distan2 = Math.Pow(resampledpt[secondpt].X - resampledpt[firstpt].X, 2) + Math.Pow(resampledpt[secondpt].Y - resampledpt[firstpt].Y, 2);
                distan2 = Math.Sqrt(distan2);
                if (distan1 != 0)// distant2 can be zero, when the two point intersect 
                {
                    // textBox1.Text += "(" + (distan2 / distan1)+"),"; // for test;
                    if (distan2 / distan1 > 0.95)
                        flg = 1;
                }

            }
            if (flg == 1)
            {
                textBox1.Text += "flg=" + flg + ",";// for test;
                return 1;

            }
            else
            {
                // textBox1.Text += "flg=" + flg + ",";// for test;
                return 0;
            }

        } // end of function "isline "

        private void GENERATEUNISTROKEPERMUTATIONS(Strokes strokes, Renderer renderer, Graphics g) // $N algorithm step 1; to consist with the above code,all the strokes are convert to pixel space 
        {


            int n = strokes.Count;
            int[] order = new int[n];
            List<int> orders = new List<int>();

            List<Strokes> unistrokes = new List<Strokes>();
            Rectangle boundingbox = strokes.GetBoundingBox();
            double w = 0;// Indicative Angle
            for (int i = 0; i < n; i++)
            {
                order[i] = i;
            }

            HeapPermute(n, order, orders);
            unistrokes = MakeUnistrokes(strokes, orders, order);
            List<Point>[] Upoint = new List<Point>[n]; // used to load each the unistrokes' Points;, every unistrokes includes n stroke
            for (int i = 0; i < n; n++)
            {
                Upoint[i] = new List<Point>();// instantiate  
            }
            //resample
            int N = 60;//N is the total points of each resampled stroke 


            // order information is not counted; 
            for (int i = 0; i < n; i++)
            {
                Point[] points = strokes[i].GetPoints();
                //ResamPlePt(renderer, g, points, N, Upoint[i]);// the sequence of the points can be reverse; 

            }
            // compute indictive angle
            //  w = IndicAngle(Upoint, n);// n is the strokes.count;

        }





        private List<Point> ResamPlePt(Renderer renderer, Graphics g, Point[] points, int N)
        {
           
            // List<Point> Resamplelist = new List<Point>();
            List<Point> Resampled = new List<Point>();
            List<Point> Resamplelist = new List<Point>();
            // 不在限制 N 的大小
            if (points.Count() == 1)
                N = 1;
            else
            {
                int count = points.Count();
                double n = (double)count;
                n= n * 0.95;
                N = (int)n;
            }
            for (int i = 0; i < points.Count(); i++)
            {
                renderer.InkSpaceToPixel(g, ref points[i]);

                // text += points[i].X + "," + points[i].Y + "/n";
                // Resampled.Add(points[i]);


            }

            //compute path
            double path = 0;
            for (int i = 0; i < points.Count() - 1; i++)
            {
                path = path + distance(points[i + 1].X, points[i + 1].Y, points[i].X, points[i].Y);
            }

            //1. D=0; shortStraw 
            double D, d;
            D = 0; d = 0;
            double S = path / N;// the smaller S, the  more precise the distance of two points that overlaped 
            Point q = new Point();

            Resamplelist.Add(points[0]);
            Resampled.Add(points[0]);
            Resampled.Add(points[0]);// if I add points[1], there may be some problem about index out of range! 
            int PtCount = points.Count();
            // for (int i = 1; i < Resampled.Count(); i++)//if use such method, the Resampled.count is changing;
            int k = 1;
            int j = 1;
            while (PtCount > 1)// because the index k started from 1, so the PtCount here shoud be larger than 1; 
            {

                d = Math.Pow(Resampled[j].X - Resampled[j - 1].X, 2) + Math.Pow(Resampled[j].Y - Resampled[j - 1].Y, 2);
                d = Math.Sqrt(d);
                if ((D + d >= S) && d != 0)
                {
                    // Point.X and Point Y is integer 
                    q.X = (int)(Resampled[j - 1].X + ((S - D) / d) * (Resampled[j].X - Resampled[j - 1].X));
                    q.Y = (int)(Resampled[j - 1].Y + ((S - D) / d) * (Resampled[j].Y - Resampled[j - 1].Y));
                    Resamplelist.Add(q);//APPEND(resampled;q)
                    Resampled[j] = q;//INSERT(points; i;q)

                    PtCount++;
                    D = 0;// Caution: D should be set to 0; otherwise there would be a dead loop
                }
                else
                {
                    D = D + d;
                    if (k < points.Count() - 1)// be careful,the index of k should not exceed point.Count()-1
                        k++;
                }

                Point pt = points[k];//

                Resampled.Add(pt);
                j++;
                PtCount--;
            }
            double[] dist = new double[4];
            string text = "";
            if (Resamplelist.Count() >= 4)
            {
                for (int i = 0; i < 4; i++)
                {
                    dist[i] = Math.Pow(Resamplelist[i + 1].X - Resamplelist[i].X, 2) + Math.Pow(Resamplelist[i + 1].Y - Resamplelist[i].Y, 2);
                    dist[i] = (int)Math.Sqrt(dist[i]);
                    text += dist[i] + "/";

                    g.DrawEllipse(Pens.Azure, points[i].X, points[i].Y, 3, 3);
                }
                textBox1.Text = "path" + path + ";" + "styluspoint count:" + Resamplelist.Count() + ";" + text;
                text = "";
            }


            for (int i = 0; i < Resamplelist.Count(); i++)
            {

                int width = 1;
                int height = 1;
                // renderer.InkSpaceToPixel(g, ref ResampledPt[i]);

                g.DrawRectangle(Pens.Azure, Resamplelist[i].X, Resamplelist[i].Y, width, height);
            }

            return Resamplelist;
        }

        // end of resample

        private double distance(int x1, int y1, int x2, int y2)
        {
            double d = 0;
            d = Math.Pow(x1 - x2, 2) + Math.Pow(y1 - y2, 2);
            d = Math.Sqrt(d);
            return d;
        }
        private void HeapPermute(int n, int[] order, List<int> orders)
        {

            if (n == 1)
            {
                for (int i = 0; i < order.Count(); i++)
                    orders.Add(order[i]);
            }

            else
            {
                for (int i = 0; i < n; i++)
                {
                    HeapPermute(n - 1, order, orders);
                    if (n % 2 != 0) //if n is odd;
                    {
                        int temp = 0;
                        temp = order[0];
                        order[0] = order[n - 1];
                        order[n - 1] = temp;
                    } // end of if;
                    else
                    {
                        int temp = 0;
                        temp = order[i];
                        order[i] = order[n - 1];
                        order[n - 1] = temp;
                    }
                }
            }

        }// end of HeapPermute

        private List<Strokes> MakeUnistrokes(Strokes strokes, List<int> orders, int[] order)
        {


            List<Strokes> unistrokes = new List<Strokes>();
            Strokes unistroke = null;

            /*  1 foreach order R in orders do
  2 for b from 0 to 2^|R| do
  3 for i from 0 to |R| do
  4 if BIT-AT(b, i) = 1 then // b’s bit at index i
  5 APPEND(unistroke, REVERSE(strokesRi))
  6 else APPEND(unistroke, strokesRi)
  7 APPEND(unistrokes, unistroke)
  8 return unistrokes*/

            int n = order.Count();
            int numberofOrder = orders.Count / n;
            int[,] AllOrder = new int[numberofOrder, n];
            for (int i = 0; i < numberofOrder; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    AllOrder[i, j] = orders[i * n + j];
                }
            }

            //  foreach order R in orders
            for (int R = 0; R < numberofOrder; R++)
            {
                for (int b = 0; b < (int)Math.Pow(2, n); b++)
                {
                    for (int i = n - 1; i >= 0; i--)
                    {
                        int b1 = b;
                        int c = (int)Math.Pow(2, i);

                        if (b1 / c > 0)
                        {
                            unistroke.Add(strokes[AllOrder[R, i]]);// don't reverse for simplicity 
                        }
                        else
                            unistroke.Add(strokes[AllOrder[R, i]]);
                        b1 = b1 - c;

                    }
                    unistrokes.Add(unistroke);

                }

            }

            return unistrokes;
        }

        private double IndicAngle(List<Point> Upoint, int n, Graphics g)
        {
            int meanX, meanY = 0;
            Point CentPoint = new Point();
            int AllptCount = 0;
            meanX = 0;
            double Angle = 0;

            for (int j = 0; j < Upoint.Count; j++)
            {
                meanX = meanX + Upoint[j].X;
                meanY = meanY + Upoint[j].Y;
                AllptCount += 1;
            }


            meanX = meanX / AllptCount;
            meanY = meanY / AllptCount;
            CentPoint.X = meanX;
            CentPoint.Y = meanY;

            Angle = Math.Atan2(Upoint[0].Y - meanY, Upoint[0].X - meanX);
            // Point[] points= Upoint.ToArray();
            g.DrawLine(Pens.Blue, Upoint[0], CentPoint);
            return Angle;
        }
        private Point[] RotateBy(Point[] points, double w)// return new rotated point
        {
            Point c = new Point();
            Point q = new Point();
            Point[] Newpoint = new Point[points.Count()];
            double x1, y1 = new double();
            c = CentroidPoint(points);
            for (int i = 0; i < points.Count(); i++)
            {
                x1 = (points[i].X - c.X) * Math.Cos(w) - (points[i].Y - c.Y) * Math.Sin(w) + c.X;
                q.X = (int)x1;
                y1 = (points[i].X - c.X) * Math.Sin(w) + (points[i].Y - c.Y) * Math.Cos(w) + c.Y;
                q.Y = (int)y1;
                Newpoint[i] = q;
            }
            return Newpoint;
        }
        
        private Point CentroidPoint(Point[] points)
        {
            Point CenterPt = new Point();

            int meanX, meanY = 0;
            meanX = 0;
            int AllptCount = 0;
            for (int i = 0; i < points.Count(); i++)
            {
                meanX = meanX + points[i].X;
                meanY = meanY + points[i].Y;
                AllptCount += 1;
            }


            meanX = meanX / AllptCount;
            meanY = meanY / AllptCount;
            CenterPt.X = meanX;
            CenterPt.Y = meanY;
            return CenterPt;
        }

        private Point[] ScaleDimTo(Point[] points, int size, double apha, Rectangle boundingbox)
        {

            Point[] newPoints = new Point[points.Count()];
            if (Math.Min(boundingbox.Width / boundingbox.Height, boundingbox.Height / boundingbox.Width) <= apha)
            {
                double Scalor = Math.Max(boundingbox.Height, boundingbox.Width);
                for (int i = 0; i < points.Count(); i++)
                {
                    newPoints[i].X = (int)(points[i].X * size / Scalor);
                    newPoints[i].Y = (int)(points[i].Y * size / Scalor);
                }
            }// end of if
            else
            {
                for (int i = 0; i < points.Count(); i++)
                {
                    newPoints[i].X = (int)(points[i].X * size / boundingbox.Width);
                    newPoints[i].Y = (int)(points[i].Y * size / boundingbox.Height);
                }
            }

            return newPoints;
        }

        private List<Point> ScaleDimToListPt(List<Point> points, int size, double apha, Rectangle boundingbox)
        {

            List<Point> newPoints = new List<Point>();
            Point newpoint = new Point();
            if (Math.Min(boundingbox.Width / boundingbox.Height, boundingbox.Height / boundingbox.Width) <= apha)
            {
                double Scalor = Math.Max(boundingbox.Height, boundingbox.Width);
                for (int i = 0; i < points.Count(); i++)
                {
                    newpoint.X = (int)(points[i].X * size / Scalor);
                    newpoint.Y = (int)(points[i].Y * size / Scalor);
                    newPoints.Add(newpoint);
                }
            }// end of if
            else
            {
                for (int i = 0; i < points.Count(); i++)
                {
                    newpoint.X = (int)(points[i].X * size / boundingbox.Width);
                    newpoint.Y = (int)(points[i].Y * size / boundingbox.Height);
                    newPoints.Add(newpoint);
                }
            }

            return newPoints;
        }



        private Point[] translateToPt(Point[] points, Point centerPt, Point OriginPt)
        {

            Point[] newPoints = new Point[points.Count()];
            for (int i = 0; i < points.Count(); i++)
            {
                newPoints[i].X = points[i].X + OriginPt.X - centerPt.X;
                newPoints[i].Y = points[i].Y + OriginPt.Y - centerPt.Y;
            }
            return newPoints;
        }

        private List<Point> translateToPtlist(List<Point> points, Point centerPt, Point OriginPt)
        {

         
            List<Point> newPointlist = new List<Point>();
            Point newpoint = new Point();
            for (int i = 0; i < points.Count(); i++)
            {
                newpoint.X = points[i].X + OriginPt.X - centerPt.X;
                newpoint.Y = points[i].Y + OriginPt.Y - centerPt.Y;
                newPointlist.Add(newpoint);
            }
            return newPointlist;
        }



        private void selectink_CheckedChanged(object sender, EventArgs e)
        {
            {
                if (selectink.Checked)
                {
                    m_InkOverlay.Enabled = false;
                    m_InkOverlay.EditingMode = InkOverlayEditingMode.Select;
                    m_InkOverlay.Handle = this.Handle;
                    m_InkOverlay.Enabled = true;
                }
                else
                {
                    m_InkOverlay.Enabled = false;
                    m_InkOverlay.EditingMode = InkOverlayEditingMode.Ink;
                    m_InkOverlay.Handle = this.Handle;
                    m_InkOverlay.Enabled = true;
                }
            }
        }

        private void loadButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDlg = new OpenFileDialog();
            fileDlg.Filter = "ISF files (*.isf)|*.isf| All files (*.*)|*.*";
            if (fileDlg.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    // Ink.Load() must work on a new (unused) ink object. 
                    // Otherwise, an exception is raised.
                    m_InkOverlay.Enabled = false;
                    m_InkOverlay.Ink = new Ink();

                    //   m_InkOverlay.Enabled = true;
                    // FILE_NAME is a class level const
                    using (FileStream FS = new FileStream(fileDlg.FileName,
                                                FileMode.Open, FileAccess.Read))
                    {
                        // read the bytes from the file
                        byte[] isf = new byte[FS.Length];
                        FS.Read(isf, 0, (int)FS.Length);
                        // and load the Ink object
                        m_InkOverlay.Ink.Load(isf);


                        // Strokes strs = m_InkOverlay.Ink.CreateStrokes();
                        FS.Close();
                    }
                    m_InkOverlay.Ink.Strokes.Add(m_InkOverlay.Ink.CreateStrokes());
                    m_InkOverlay.Enabled = true;
                }
                catch (Exception)
                {
                    MessageBox.Show(this, "Unable to load the image file!", "Load Image Failed");
                }

            }
        }

        private void DeleteBotton_Click(object sender, EventArgs e)
        {
            m_InkOverlay.Enabled = false;
            m_InkOverlay.EditingMode = InkOverlayEditingMode.Delete;
            m_InkOverlay.Handle = this.Handle;
            m_InkOverlay.Enabled = true;
        }

        private void DeleteBotton_Click_1(object sender, EventArgs e)
        {
            m_InkOverlay.Enabled = false;
            m_InkOverlay.EditingMode = InkOverlayEditingMode.Delete;
            m_InkOverlay.Handle = this.Handle;
            m_InkOverlay.Enabled = true;

            //Strokes strs = m_InkOverlay.Selection();
            //m_InkOverlay.Ink.Load(

        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            SaveFileDialog savedlg = new SaveFileDialog();

            savedlg.Filter = "data files (*.data)|*.data| All files (*.*)|*.*";
            if (savedlg.ShowDialog() == DialogResult.OK)
            {
                //Stream stream = savedlg.OpenFile();
                try
                {

                    using (FileStream fs = new FileStream(savedlg.FileName, FileMode.Create, FileAccess.Write))
                    {


                        BinaryFormatter b = new BinaryFormatter();
                        b.Serialize(fs, UnistrokePt);// save the unistrokePt that is the resampled and reprocessing strokes points
                        fs.Close();

                    }// save template to file



                }
                catch (Exception)
                {
                    MessageBox.Show(this, "Unable to load the image file!", "save file Failed");
                }
                //stream.Close();

            }
            //  m_InkOverlay.Enabled = true;
        }

        // Load template
        private void LoadTemplate(string filename, Strokes RawTemplateStrokes)
        {
            //load template 
            //  Strokes str= m_InkOverlay.Selection;
            //  FileStream fs = new FileStream("A:\\Pen_based\\templates\\Atemplate.isf", FileMode.Open, FileAccess.ReadWrite);

            FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.ReadWrite);
            m_InkOverlay.Enabled = false;
            m_InkOverlay.Ink = new Ink();
            m_InkOverlay.Enabled = true;
            // read the bytes from the file
            byte[] isf = new byte[fs.Length];
            fs.Read(isf, 0, (int)fs.Length);
            // and load the Ink object
            m_InkOverlay.Ink.Load(isf);
            //load templates strokes to a strokes variable
            RawTemplateStrokes = m_InkOverlay.Ink.Strokes;


            m_InkOverlay.Ink.DeleteStrokes();
            //  return RawTemplateStrokes;
        }

        private void TemplateStrokToPt(Strokes RawTemplateStrokes, List<Point>[] templatePoints, Renderer renderer, Graphics g)
        {


            int index = 0; // index of the 

            // load the stokes into points sequence 
            foreach (Stroke str in RawTemplateStrokes)
            {
                if (index < templatePoints.Count())
                {
                    Point[] points = str.GetPoints();
                    for (int i = 0; i < points.Count(); i++)
                    {
                        templatePoints[index].Add(points[i]);
                    }


                }
                index += 1;
            }// end load the strokes into points sequence 

            for (int i = 0; i < templatePoints.Count(); i++)
            {
                Point[] points = templatePoints[i].ToArray();
                templatePoints[i] = ResamPlePt(renderer, g, points, 60);


            }
        }
        private void PaintPoints(object sender, System.Windows.Forms.PaintEventArgs pe)
        {
            Graphics g = pe.Graphics;
            Renderer renderer = new Renderer();
            // TemplateStrokToPt(RawTemplate, templatePoints, renderer, g);

        }

        private void Reconize_Click(object sender, EventArgs e)
        {

        }

        // return the distance between two points sequence 
        private double path_distance(Point[] A, Point[] B)
        {
            double d = 0;
            for (int i = 0; i < Math.Min(A.Count(), B.Count()); i++)
            {
                d = d + distance(A[i].X, A[i].Y, B[i].X, B[i].Y);
            }
            return (d / Math.Min(A.Count(), B.Count()));


        }

        private double distance_at_angle(Point[] points, Point[] T, double Theta)
        {
            Point[] newpoints = RotateBy(points, Theta);
            double d = path_distance(newpoints, T);
            return d;
        }

        private double Distance_at_BestAngle(Point[] points, Point[] T, double Theta_a, double Theta_b, double angleThreshold)
        {
            double lamda = 1 / 2 * (-1 + Math.Sqrt(5));
            double x1 = Theta_a * lamda + (1 - lamda) * Theta_b;
            double f1 = distance_at_angle(points, T, x1);
            double x2 = Theta_a * (1 - lamda) + lamda * Theta_b;
            double f2 = distance_at_angle(points, T, x2);
            while (Math.Abs(Theta_a - Theta_b) > angleThreshold)
            {
                if (f1 < f2)
                {
                    Theta_b = x2;
                    x2 = x1;
                    f2 = f1;
                    x1 = lamda * Theta_a + (1 - lamda) * Theta_b;
                    f1 = distance_at_angle(points, T, x1);

                }
                else
                {
                    Theta_a = x1;
                    x1 = x2;
                    f1 = f2;
                    x2 = (1 - lamda) * Theta_a + lamda * Theta_b;
                    f2 = distance_at_angle(points, T, x2);

                }

            }// while
            return Math.Min(f1, f2);
        }

        // compare each stroke in a symbol strokes with template's seperately
        private double recognize(Point[] points, Point[] T)
        {
            double d = Distance_at_BestAngle(points, T, -45, 45, 2);
            return d;
        }

        private List<Point>[] LoadDeserializePt(List<Point>[] templatepoints, string filename)
        {
            using (FileStream fs1 = new FileStream(filename, FileMode.Open))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                templatepoints = (List<Point>[])formatter.Deserialize(fs1);
            }
            //   for (int j1 = 0; j1 < templatepoints.Count(); j1++)
            // {
            //   for (int r1 = 0; r1 < templatepoints[j1].Count(); r1++)
            //     g.DrawRectangle(Pens.Yellow, templatepoints[j1][r1].X, templatepoints[j1][r1].Y, 2, 2);
            //    }

            return templatepoints;


        }// load templates' points

        private int recognizeSymb(List<Stroke> selectionstrks, Rectangle boundingbox, Graphics g, int N, Renderer renderer)
        {
            List<Point> selectionlist = new List<Point>();

            if (selectionstrks.Count > 0)
            {
                UnistrokePt = new List<Point>[selectionstrks.Count];

                for (int j = 0; j < selectionstrks.Count; j++)
                {
                    UnistrokePt[j] = new List<Point>();
                    Stroke str = selectionstrks[j];


                    Point[] points = str.GetPoints();
                    selectionlist = ResamPlePt(renderer, g, points, N);
                    Point[] resamplepoints = selectionlist.ToArray();
                    double indictiveangle = IndicAngle(selectionlist, selectionlist.Count(), g);

                    // resamplepoints= RotateBy(resamplepoints, -indictiveangle);
                    //  Rectangle boundbox = selectionstrks.GetBoundingBox();// bounding box for all the unistroke
                    Rectangle boundbox = boundingbox;
                    resamplepoints = ScaleDimTo(resamplepoints, 960, 0.3, boundbox);
                    Point centpoint = CentroidPoint(resamplepoints);
                    Point originpoint = new Point();

                    originpoint.X = 10;
                    originpoint.Y = 10;
                    // translate to originpoint 
                    resamplepoints = translateToPt(resamplepoints, centpoint, originpoint);

                    for (int i = 0; i < resamplepoints.Count(); i++)
                    {
                        UnistrokePt[j].Add(resamplepoints[i]);

                    }
                    // for (int i = 0; i < resamplepoints.Count(); i++)
                    //    g.DrawRectangle(Pens.Red, resamplepoints[i].X, resamplepoints[i].Y, 2, 2);
                }// foreach
                // selection mode is trigger by select the strokes and click the bounding box of the selection
                //  for (int i = 0; i < UnistrokePt.Count(); i++)
                // {
                //   for (int j = 0; j < UnistrokePt[i].Count(); j++)
                //     g.DrawRectangle(Pens.Red, UnistrokePt[i][j].X, UnistrokePt[i][j].Y, 2, 2);
                //}
                // recognition by comparing the selected strokes with the template strokes
                double distant = 100;
                int cl = -1;// the class the strokes belong to

                // recognize the selected strokes
                // double d = 0;
                double dmin = 100;
                int templateIndex = 0;
                for (int t = 0; t < 12; t++)// 12 templates
                {
                    double d = 0;
                    if (templateGroup[t].Count() == UnistrokePt.Count())
                    {
                        templatePoints = templateGroup[t];
                        for (int i = 0; i < templatePoints.Count(); i++)
                        {
                            Point[] points = UnistrokePt[i].ToArray();
                            Point[] T = templatePoints[i].ToArray();
                            d = d + path_distance(points, T);
                        }
                        if (d < dmin)
                        {
                            templateIndex = t;
                            dmin = d;
                        }


                        //    textBox1.Clear();
                        //  textBox1.Text = d.ToString();
                        // distant = d;
                        //cl = 0;//templates points0 
                    }
                }// for

                textBox1.Clear();

                switch (templateIndex)
                {
                    case 0:
                        textBox1.Text = "1";
                        break;
                    case 1:
                        textBox1.Text = "2";
                        break;
                    case 2:
                        textBox1.Text = "3";
                        break;
                    case 3:
                        textBox1.Text = "4";
                        break;
                    case 4:
                        textBox1.Text = "5";
                        break;
                    case 5:
                        textBox1.Text = "6";
                        break;
                    case 6:
                        textBox1.Text = "7";
                        break;
                    case 7:
                        textBox1.Text = "8";
                        break;
                    case 8:
                        textBox1.Text = "9";
                        break;
                    case 9:
                        textBox1.Text = "plus";
                        break;
                    case 10:
                        textBox1.Text = "Multiply";
                        break;
                    case 11:
                        textBox1.Text = "Sqrt";
                        break;
                }
                return (templateIndex);
            }
            return 0;
        }





        // 
        private bool IsOperator(char ch)
        {
            string ops = "+-*/";// include
            for (int i = 0; i < ops.Length; i++)
            {

                if (ch == ops[i])
                    return true;
            }
            return false;
        }

        //////////////////////////////////////////////////////////////////////////
        // 比较两个操作符的优先级

        private int Precedence(char op1, char op2)
        {
            if (op1 == '(')
            {
                return -1;
            }

            if (op1 == '+' || op1 == '-')
            {
                if (op2 == '*' || op2 == '/')
                {
                    return -1;
                }
                else
                {
                    return 0;
                }
            }

            if (op1 == '*' || op1 == '/')
            {
                if (op2 == '+' || op2 == '-')
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
            return 0;// defalt
        }

        private string inFix2PostFix(char[] inFix, char[] postFix)// char* may be equal to char[] in C#
        {
            int j = 0, len;
            char c;
            Stack<char> st = new Stack<char>();

            len = inFix.Count();

            for (int i = 0; i < len; i++)
            {
                c = inFix[i];

                if (c == '(')
                    st.Push(c);
                else if (c == ')')
                {
                    while (st.Peek() != '(')
                    {
                        postFix[j++] = st.Peek();
                        st.Pop();
                    }
                    st.Pop();
                }
                else
                {
                    if (!IsOperator(c))
                        st.Push(c);
                    else
                    {
                        while (st.Count() > 0
                               && Precedence(st.Peek(), c) >= 0)
                        {
                            postFix[j++] = st.Peek();
                            st.Pop();
                        }
                        st.Push(c);
                    }
                }
            }

            while (st.Count() > 0)
            {
                postFix[j++] = st.Peek();
                st.Pop();
            }
            postFix[j] = (char)0;
            // return string
            string postfixstr = "";
            for (int i = 0; i < j + 1; i++)
                postfixstr += postFix[i];
            return postfixstr;
        }

        //////////////////////////////////////////////////////////////////////////
        // 后缀表达式求值程序
        private int postFixEval(char[] postFix) // in the original code, the postfix is char*
        {
            Stack<int> st = new Stack<int>();
            //Stack<int> result_val = new Stack<int>();
            int len = postFix.Length;
            char c;
            int InterResult = 0;
            for (int i = 0; i < len - 1; i++)// the last element of the post fix is 0
            {
                c = postFix[i];
                if (IsOperator(c) == false)
                {

                    InterResult = c - '0';
                    st.Push(InterResult);
                }
                else     // is operator        
                {
                    int op1, op2;
                    int val = 0;

                    op1 = st.Peek();
                    st.Pop();
                    op2 = st.Peek();
                    st.Pop();

                    switch (c)
                    {
                        case '+':
                            val = op1 + op2;
                            break;
                        case '-':
                            val = op2 - op1;
                            break;
                        case '*':
                            val = op1 * op2;
                            break;
                        case '/':
                            val = op2 / op1;
                            break;
                    }

                    st.Push(val);
                }
            }
            int val1 = st.Peek();
            return val1;
        }


        // save inkstrokes
        private void saveinkbtn_Click(object sender, EventArgs e)
        {
            SaveFileDialog savedlg = new SaveFileDialog();

            savedlg.Filter = "data files (*.isf)|*.isf| All files (*.*)|*.*";
            if (savedlg.ShowDialog() == DialogResult.OK)
            {
                //Stream stream = savedlg.OpenFile();
                try
                {

                    using (FileStream fs = new FileStream(savedlg.FileName, FileMode.Create, FileAccess.ReadWrite))
                    {
                     byte [] theSavedInk =m_InkOverlay.Ink.Save(PersistenceFormat.InkSerializedFormat);
                       
                        //Strokes str = new Strokes();
                       // BinaryFormatter b = new BinaryFormatter();
                     fs.Write(theSavedInk, 0, theSavedInk.Count());// save the unistrokePt that is the resampled and reprocessing strokes points
                        fs.Close();

                    }// save template to file



                }
                catch (Exception exc)
                {
                    MessageBox.Show(exc.Message );
                }
                //stream.Close();

            }

        }


       

        private void Loadinkbtn_Click_1(object sender, EventArgs e)
        {

            OpenFileDialog dlg = new OpenFileDialog();
       //     dlg.CheckFileExists = true;
            dlg.Filter = "data files (*.isf)|*.isf| All files (*.*)|*.*"; 
            m_InkOverlay.Enabled = false;
            m_InkOverlay.Ink = new Ink();
            m_InkOverlay.Enabled = true;
            if( dlg.ShowDialog() == DialogResult.OK) // to prevent the exception 
            try
            {
                using (FileStream FS = new FileStream(dlg.FileName, FileMode.Open, FileAccess.Read))
                {
                    byte[] isf = new byte[FS.Length];
                    FS.Read(isf, 0, (int)FS.Length);
                    // and load the Ink object
                    m_InkOverlay.Ink.Load(isf);
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
            }
            loadstrokes = m_InkOverlay.Ink.Strokes;
            loadflag = 1;

        }

        private peopleMd CreatPlMd( Strokes selectionstrks, Renderer renderer, Graphics g )
        {
            int strkcount = selectionstrks.Count;
            Stroke[] frntchairstks = new Stroke[selectionstrks.Count];
            for (int i = 0; i < selectionstrks.Count; i++)
            {
                frntchairstks[i] = selectionstrks[i];
            }

            // Array.Sort(ExpressionU, (x, y) => (x.boundingbox.Left).CompareTo(y.boundingbox.Left));//the order of (x.boundingbox.Left).CompareTo(y.boundingbox.Left)
            Array.Sort(frntchairstks, (x, y) => (x.GetBoundingBox().Top + x.GetBoundingBox().Bottom).CompareTo(y.GetBoundingBox().Top + y.GetBoundingBox().Bottom));// sort according to the bounding box's center point's y from large to small 
            Stroke leg1, leg2, hand1, hand2, head, back;
            if (frntchairstks[strkcount - 2].GetBoundingBox().Left < frntchairstks[strkcount - 1].GetBoundingBox().Left)
            {
               leg1 = frntchairstks[strkcount - 2];
               leg2 = frntchairstks[strkcount - 1];
            }
            else
            {
                leg1 = frntchairstks[strkcount - 1];
                leg2 = frntchairstks[strkcount - 2];
            }
            
            head  = frntchairstks[0];
            back  = frntchairstks[3];
            hand1 = frntchairstks[1];
            hand2 = frntchairstks[2];
            float[] a1 = new float[3];
            int back_index = 3;
            int hand1_index = 1;
            int hand2_index = 2;
            a1[0] = back.GetBoundingBox().Left;
            a1[1] = hand1.GetBoundingBox().Left;
            a1[2] = hand2.GetBoundingBox().Left;
            Array.Sort(a1, (y, x) => (x.CompareTo(y)));
            if (a1[1] == back.GetBoundingBox().Left)
            {
                back_index = 3;
                
            }
            else if (a1[1] == hand1.GetBoundingBox().Left)
            {
                back_index = 1;
                hand1_index = 3;
            }
            else
            {
                back_index = 2;
                hand2_index = 3;
            }
            back = frntchairstks[back_index];
            hand1 = frntchairstks[hand1_index];
            hand2 = frntchairstks[hand2_index];
            List<Point> backPt = ResamPlePt(renderer, g, back.GetPoints(), back.GetPoints().Count() / 2);
            List<Point> HeadPt = ResamPlePt(renderer, g, head.GetPoints(), head.GetPoints().Count() / 2);
            List<Point> hand1Pt = ResamPlePt(renderer, g, hand1.GetPoints(), hand1.GetPoints().Count() / 2);
            List<Point> hand2Pt = ResamPlePt(renderer, g, hand2.GetPoints(), hand2.GetPoints().Count() / 2);
            List<Point> leg1Pt = ResamPlePt(renderer, g, leg1.GetPoints(), leg1.GetPoints().Count() / 2);
            List<Point> leg2Pt = ResamPlePt(renderer, g, leg2.GetPoints(), leg2.GetPoints().Count() / 2);
            peopleMd peoplmd1 = new peopleMd(leg1Pt, leg2Pt, backPt, hand1Pt, hand2Pt, HeadPt);

            // save to binary file
            using (FileStream fs = new FileStream("peoplemd1.data", FileMode.Create, FileAccess.Write))
            {


                BinaryFormatter b = new BinaryFormatter();
                b.Serialize(fs,peoplmd1);
                fs.Close();

            }

            // load data structure from file 
            //peopleMd peoplemd = new peopleMd();
            using (FileStream fs1 = new FileStream("peoplemd1.data", FileMode.Open))
            {
                BinaryFormatter formatter = new BinaryFormatter();
               pubicPlmd = (peopleMd)formatter.Deserialize(fs1);
            }

            return pubicPlmd;

        }
        private FrontChair PlSitOnFrChair(peopleMd plmd, FrontChair frntCh)
        {
            Point referPt1 = CentroidPoint(frntCh.chairshaft.ToArray());// center point of the the chair cross shaft;
            Point referPt2= new Point();
            Point[] backpt = plmd.back.ToArray();
            Array.Sort(backpt, (x, y) => (x.Y).CompareTo(y.Y));// sort in decent order;
            referPt2 = backpt[(backpt.Count() * 2 / 3)]; // take the point in the 2/3 location of the back as reference point; 
       // move the chair so that the chair will be sitted by the peoplemd 

            List<Point> newChBk = translateToPtlist(frntCh.chairback, referPt1, referPt2);
            List<Point> newChSeat = translateToPtlist(frntCh.chairseat, referPt1, referPt2);
            List<Point> newChleg1 = translateToPtlist(frntCh.chairleg_1, referPt1, referPt2);
            List<Point> newChleg2 = translateToPtlist(frntCh.chairleg_2, referPt1, referPt2);
            List<Point> newChShaft = translateToPtlist(frntCh.chairshaft, referPt1, referPt2);
            FrontChair newfrntch = new FrontChair(newChShaft, newChleg1, newChleg2, newChBk, newChSeat);
            return newfrntch;
        }

        // print people sit on a side view chair 
        private void  PlsitOnSdChair()
        {
            MessageBox.Show("please choose a side view people md");
            OpenFileDialog fileDlg = new OpenFileDialog();
            fileDlg.Filter = "data files (*.data)|*.data| All files (*.*)|*.*";
            if (fileDlg.ShowDialog() == DialogResult.OK)
            {
                try
                {

                    using (FileStream FS = new FileStream(fileDlg.FileName,
                                                FileMode.Open, FileAccess.Read))
                    {
                        BinaryFormatter formatter = new BinaryFormatter();
                        pubicPlmd = (peopleMd) formatter.Deserialize(FS);
                    }

                }
                catch (Exception)
                {
                    MessageBox.Show(this, "Unable to load the image file!", "Load Image Failed");
                }

            }
            // end of choosing a people model 
            MessageBox.Show("please choose a side view Chair md");
            // choose a chair model
            OpenFileDialog fileDlg1 = new OpenFileDialog();
            fileDlg1.Filter = "data files (*.data)|*.data| All files (*.*)|*.*";
            if (fileDlg1.ShowDialog() == DialogResult.OK)
            {
                try
                {

                    using (FileStream FS1 = new FileStream(fileDlg1.FileName,
                                                FileMode.Open, FileAccess.Read))
                    {
                        BinaryFormatter formatter1 = new BinaryFormatter();
                       publicsideChair = (SideViewChair) formatter1.Deserialize(FS1);
                    }

                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }

            }

            
            // end of choosing a chair model 
            MessageBox.Show("Great! begin to show the people sit on the chair");

            Point referPt1 = CentroidPoint(publicsideChair.chairseat.ToArray());// center point of the the chair cross shaft;
            Point referPt2 = new Point();
            Point[] backpt = pubicPlmd.back.ToArray();
            Array.Sort(backpt, (x, y) => (x.Y).CompareTo(y.Y));// sort in decent order;
            referPt2 = backpt[(backpt.Count()-1)]; // take the point in the lowest point of the back as reference point; 
            // move the chair so that the chair will be sitted by the peoplemd 

            List<Point> newChBk = translateToPtlist(publicsideChair.chairback, referPt1, referPt2);
            List<Point> newChSeat = translateToPtlist(publicsideChair.chairseat, referPt1, referPt2);
            List<Point> newChleg1 = translateToPtlist(publicsideChair.chairleg_1, referPt1, referPt2);
            List<Point> newChleg2 = translateToPtlist(publicsideChair.chairleg_2, referPt1, referPt2);
            //List<Point> newChShaft = translateToPtlist(frntCh.chairshaft, referPt1, referPt2);
            SideViewChair newsdch = new SideViewChair(newChleg1, newChleg2, newChBk, newChSeat);
            printPlmd(pubicPlmd);
            PrintSdChair(newsdch);
        }
        private void printFrCh(FrontChair frntchair1)
        {
            
            System.Drawing.Graphics g = this.CreateGraphics();
            for (int i1=0;i1<frntchair1.chairleg_1.Count();i1++)
                        g.DrawRectangle(Pens.Red, frntchair1.chairleg_1[i1].X, frntchair1.chairleg_1[i1].Y, 2, 2);
                    for (int i1 = 0; i1 < frntchair1.chairleg_2.Count(); i1++)
                        g.DrawRectangle(Pens.Green, frntchair1.chairleg_2[i1].X, frntchair1.chairleg_2[i1].Y, 2, 2);
                    for (int i1 = 0; i1 < frntchair1.chairback.Count(); i1++)
                        g.DrawRectangle(Pens.Blue, frntchair1.chairback[i1].X, frntchair1.chairback[i1].Y, 2, 2);
                    for (int i1 = 0; i1 < frntchair1.chairseat.Count(); i1++)
                        g.DrawRectangle(Pens.Yellow, frntchair1.chairseat[i1].X, frntchair1.chairseat[i1].Y, 2, 2);
                    for (int i1 = 0; i1 < frntchair1.chairshaft.Count(); i1++)
                        g.DrawRectangle(Pens.YellowGreen, frntchair1.chairshaft[i1].X, frntchair1.chairshaft[i1].Y, 2, 2);
                    Point referPt1 = CentroidPoint(frntchair1.chairshaft.ToArray());// center point of the cross chairshaft 
                    g.DrawRectangle(Pens.Red, referPt1.X, referPt1.Y, 2, 2);
        }

        private void printSdCh(SideViewChair frntchair1)
        {
            System.Drawing.Graphics g = this.CreateGraphics();
            for (int i1 = 0; i1 < frntchair1.chairleg_1.Count(); i1++)
                g.DrawRectangle(Pens.Red, frntchair1.chairleg_1[i1].X, frntchair1.chairleg_1[i1].Y, 2, 2);
            for (int i1 = 0; i1 < frntchair1.chairleg_2.Count(); i1++)
                g.DrawRectangle(Pens.Green, frntchair1.chairleg_2[i1].X, frntchair1.chairleg_2[i1].Y, 2, 2);
            for (int i1 = 0; i1 < frntchair1.chairback.Count(); i1++)
                g.DrawRectangle(Pens.Blue, frntchair1.chairback[i1].X, frntchair1.chairback[i1].Y, 2, 2);
            for (int i1 = 0; i1 < frntchair1.chairseat.Count(); i1++)
                g.DrawRectangle(Pens.Yellow, frntchair1.chairseat[i1].X, frntchair1.chairseat[i1].Y, 2, 2);
            
        }
        private void printPlmd(peopleMd plmd)
        {
            System.Drawing.Graphics g = this.CreateGraphics();
            for (int i1 = 0; i1 < plmd.leg_1.Count(); i1++)
                g.DrawRectangle(Pens.Red, plmd.leg_1[i1].X, plmd.leg_1[i1].Y, 2, 2);
            for (int i1 = 0; i1 < plmd.leg_2.Count(); i1++)
                g.DrawRectangle(Pens.Yellow, plmd.leg_2[i1].X, plmd.leg_2[i1].Y, 2, 2);
            for (int i1 = 0; i1 < plmd.head.Count(); i1++)
                g.DrawRectangle(Pens.Blue, plmd.head[i1].X, plmd.head[i1].Y, 2, 2);
            for (int i1 = 0; i1 < plmd.back.Count(); i1++)
                g.DrawRectangle(Pens.Yellow, plmd.back[i1].X, plmd.back[i1].Y, 2, 2);
            for (int i1 = 0; i1 < plmd.hand_1.Count(); i1++)
                g.DrawRectangle(Pens.Green, plmd.hand_1[i1].X, plmd.hand_1[i1].Y, 2, 2);
            for (int i1 = 0; i1 < plmd.hand_2.Count(); i1++)
                g.DrawRectangle(Pens.Green, plmd.hand_2[i1].X, plmd.hand_2[i1].Y, 2, 2);
            Point[] backpt = plmd.back.ToArray();
            Point referPt2 = new Point();
            Array.Sort(backpt, (x, y) => (x.Y).CompareTo(y.Y));// sort in decent order;
            referPt2 = backpt[(backpt.Count() * 3 / 4)];
            g.DrawRectangle(Pens.Green, referPt2.X, referPt2.Y, 3, 3);

        }

        // printscene
        /*
            SideViewChair public_LeftSdVCh = new SideViewChair(); // ok
            SideViewChair public_RightSdVCh = new SideViewChair();//ok 
            
            peopleMd public_Stand_Plmd = new peopleMd();//ok
            
            peopleMd public_Sit_RightPlmd = new peopleMd();//ok
            peopleMd public_Sit_LeftPlmd = new peopleMd();//ok
                     public_RightFrntChair         */ // ok
        private Point CenterPtof_Rectangle(Rectangle rect)
        {
            Point point1 = new Point();
            point1.X = (rect.Left + rect.Right) / 2;
            point1.Y = (rect.Top + rect.Bottom) / 2;

            return point1; 
        }

        private void PrintScene_Click(object sender, EventArgs e)
        {
          

            this.Refresh(); // clear the form for redraw something
           /* print plmd*/
            PeopleInfo plmdinfo= new PeopleInfo();
           // initialize public scene 

            publicScene = new scene();
            publicScene.PeopleGroup = new List<peopleMd>();
            publicScene.tables = new List<table>();
            publicScene.sideViewChairs = new List<SideViewChair>();
            publicScene.frontchairs = new List<FrontChair>();
            // loading plmd
            Point referpt = new Point();
            peopleMd plmd = new peopleMd();

            OpenFileDialog fileDlg = new OpenFileDialog();
            fileDlg.Filter = "data files (*.data)|*.data| All files (*.*)|*.*";
            if (fileDlg.ShowDialog() == DialogResult.OK)
            {
                try
                {

                    using (FileStream fs1 = new FileStream(fileDlg.FileName, FileMode.Open))
                    {
                        BinaryFormatter formatter = new BinaryFormatter();
                        publicSceneStructure = (SceneSturcture)formatter.Deserialize(fs1);
                    }

                }
                catch (Exception)
                {
                    MessageBox.Show(this, "Unable to load the image file!", "Load Image Failed");
                }

            }
           

            for (int i = 0; i < publicSceneStructure.PeopleGrpInfo.Count(); i++)
            {
                // load one of three kinds peoplemd left sit, right sit, stand 
               plmdinfo= publicSceneStructure.PeopleGrpInfo[i];
               if (publicSceneStructure.PeopleGrpInfo[i].viewside == 0&& plmdinfo.sitOrstand==0)
               {
                   plmd = public_Sit_LeftPlmd;
                   referpt = CenterPtof_Rectangle(boundingbox_plmd(plmd));
                   plmd = MovePlmd(plmd,referpt , plmdinfo.CentLocation);
                   publicScene.PeopleGroup.Add(plmd);


                  
               }
               if (publicSceneStructure.PeopleGrpInfo[i].viewside == 1 && plmdinfo.sitOrstand == 0)
               {
                   plmd = public_Sit_RightPlmd;
                   referpt = CenterPtof_Rectangle(boundingbox_plmd(plmd));
                   plmd = MovePlmd(plmd, referpt, plmdinfo.CentLocation);
                  
                   publicScene.PeopleGroup.Add(plmd);
               }
               if (publicSceneStructure.PeopleGrpInfo[i].sitOrstand == 1)
               {
                   plmd = public_Stand_Plmd;
                   referpt = CenterPtof_Rectangle(boundingbox_plmd(plmd));
                   plmd = MovePlmd(plmd, referpt, plmdinfo.CentLocation);
                   publicScene.PeopleGroup.Add(plmd);
               
                
               }
            }
            // end of loading peoplemd 

            // loading front chair 
            FrontChair frntchair = new FrontChair();
            for (int i = 0; i < publicSceneStructure.FrntChairsInfo.Count(); i++)
            {
                frntchair = public_RightFrntChair;
                referpt = CenterPtof_Rectangle(boundboxofFrontChair(frntchair));
                frntchair = MovefrntChair(frntchair, referpt, publicSceneStructure.FrntChairsInfo[i].centerLocation);
                publicScene.frontchairs.Add(frntchair);
            }

            // loading side view chair 
            SdvchairInfo sideviewchairInfo = new SdvchairInfo();
            SideViewChair sdvchair = new SideViewChair();
            for (int i = 0; i < publicSceneStructure.SdvChairsInfo.Count(); i++)
            {

                sideviewchairInfo = publicSceneStructure.SdvChairsInfo[i];
               
            if (sideviewchairInfo.viewside == 0)
            {
                sdvchair = public_LeftSdVCh;
                referpt = CenterPtof_Rectangle(Boundingbox_sdvchair(sdvchair));
                sdvchair = MoveSdChair(sdvchair, referpt, sideviewchairInfo.centerLocation);


                publicScene.sideViewChairs.Add(sdvchair);
            }
            else
            {
                sdvchair = public_RightSdVCh;
                referpt = CenterPtof_Rectangle(Boundingbox_sdvchair(sdvchair));
                sdvchair = MoveSdChair(sdvchair, referpt, sideviewchairInfo.centerLocation);
                publicScene.sideViewChairs.Add(sdvchair);
            }

            }// end of loading side view chair 
            table tb= new table();
            for (int i = 0; i < publicSceneStructure.tablesInfo.Count(); i++)
            {
                tb = PublicTable;
                referpt=CenterPtof_Rectangle(boundboxofTable(tb));
                tb = MoveTable(tb, referpt, publicSceneStructure.tablesInfo[i].Centlocation);
                publicScene.tables.Add(tb);
            }

            print_publice_scene();
        }

        // print front view Chair 
        private void button1_Click(object sender, EventArgs e)
        {


            this.Refresh();// clear screen and redraw something 
            OpenFileDialog fileDlg = new OpenFileDialog();
            fileDlg.Filter = "data files (*.data)|*.data| All files (*.*)|*.*";
            if (fileDlg.ShowDialog() == DialogResult.OK)
            {
                try
                {

                    using (FileStream FS = new FileStream(fileDlg.FileName,
                                                FileMode.Open, FileAccess.ReadWrite))
                    {
                        BinaryFormatter formatter = new BinaryFormatter();
                        publicfrntChair = (FrontChair)formatter.Deserialize(FS);
                    }

                }
                catch (Exception)
                {
                    MessageBox.Show(this, "Unable to load the image file!", "Load Image Failed");
                }

            }
            // autimatically copy the selected front view chair to the sidechairTamplate1 in the debug/bin derectory  
            using (FileStream fs = new FileStream("Chairtemplate1.data", FileMode.Create, FileAccess.Write))
            {


                BinaryFormatter b = new BinaryFormatter();
                b.Serialize(fs, publicfrntChair);// save the unistrokePt that is the resampled and reprocessing strokes points
                fs.Close();

            }// save template to file

            printFrCh(publicfrntChair);
         
        }
        // Save PLMD btn   
        private void button2_Click(object sender, EventArgs e)
        {
            SaveFileDialog savedlg = new SaveFileDialog();

            savedlg.Filter = "data files (*.data)|*.data| All files (*.*)|*.*";
            if (savedlg.ShowDialog() == DialogResult.OK)
            {
                //Stream stream = savedlg.OpenFile();
                try
                {

                    using (FileStream fs = new FileStream(savedlg.FileName, FileMode.Create, FileAccess.Write))
                    {


                        BinaryFormatter b = new BinaryFormatter();
                        b.Serialize(fs, pubicPlmd);// save the unistrokePt that is the resampled and reprocessing strokes points
                        fs.Close();

                    }// save template to file



                }
                catch (Exception)
                {
                    MessageBox.Show(this, "Unable to load the image file!", "save file Failed");
                }
                //stream.Close();

            }
        }
        // select, load and print people model: printPlmd
        private void button3_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDlg = new OpenFileDialog();
            fileDlg.Filter = "data files (*.data)|*.data| All files (*.*)|*.*";
            if (fileDlg.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    
                    using (FileStream FS = new FileStream(fileDlg.FileName,
                                                FileMode.Open, FileAccess.Read))
                    {
                        BinaryFormatter formatter = new BinaryFormatter();
                        pubicPlmd = (peopleMd) formatter.Deserialize(FS);
                    }
                
                }
                catch (Exception)
                {
                    MessageBox.Show(this, "Unable to load the image file!", "Load Image Failed");
                }

            }

            using (FileStream fs = new FileStream("peopleTemplate1.data", FileMode.Create, FileAccess.Write))
            {


                BinaryFormatter b = new BinaryFormatter();
                b.Serialize(fs, pubicPlmd);// save the unistrokePt that is the resampled and reprocessing strokes points
                fs.Close();

            }// save template to file

            this.Refresh();
            printPlmd(pubicPlmd);

        }

        private void SaveChair_Click(object sender, EventArgs e)
        {
            if( m_InkOverlay.Selection.Count==5)  //frontview Chair
            {
                SaveFileDialog savedlg = new SaveFileDialog();

                savedlg.Filter = "data files (*.data)|*.data| All files (*.*)|*.*";
                if (savedlg.ShowDialog() == DialogResult.OK)
                {
                    //Stream stream = savedlg.OpenFile();
                    try
                    {

                        using (FileStream fs = new FileStream(savedlg.FileName, FileMode.Create, FileAccess.Write))
                        {


                            BinaryFormatter b = new BinaryFormatter();
                            b.Serialize(fs, publicfrntChair);// save the unistrokePt that is the resampled and reprocessing strokes points
                            fs.Close();

                        }// save template to file



                    }
                    catch (Exception)
                    {
                        MessageBox.Show(this, "Unable to load the image file!", "save file Failed");
                    }
                    //stream.Close();

                }
            }
            if (m_InkOverlay.Selection.Count == 4)// sideview chair
            {
                SaveFileDialog savedlg = new SaveFileDialog();

                savedlg.Filter = "data files (*.data)|*.data| All files (*.*)|*.*";
                if (savedlg.ShowDialog() == DialogResult.OK)
                {
                    //Stream stream = savedlg.OpenFile();
                    try
                    {

                        using (FileStream fs = new FileStream(savedlg.FileName, FileMode.Create, FileAccess.Write))
                        {


                            BinaryFormatter b = new BinaryFormatter();
                            b.Serialize(fs, publicsideChair);// save the unistrokePt that is the resampled and reprocessing strokes points
                            fs.Close();

                        }// save template to file



                    }
                    catch (Exception)
                    {
                        MessageBox.Show(this, "Unable to load the image file!", "save file Failed");
                    }
                    //stream.Close();

                }
            }
        }
        private void PrintSdChair(SideViewChair sidechair1)
        {
            System.Drawing.Graphics g = this.CreateGraphics();
            for (int i1 = 0; i1 < sidechair1.chairleg_1.Count(); i1++)
                g.DrawRectangle(Pens.Red, sidechair1.chairleg_1[i1].X, sidechair1.chairleg_1[i1].Y, 2, 2);
            for (int i1 = 0; i1 < sidechair1.chairleg_2.Count(); i1++)
                g.DrawRectangle(Pens.Green, sidechair1.chairleg_2[i1].X, sidechair1.chairleg_2[i1].Y, 2, 2);
            for (int i1 = 0; i1 < sidechair1.chairback.Count(); i1++)
                g.DrawRectangle(Pens.Blue, sidechair1.chairback[i1].X, sidechair1.chairback[i1].Y, 2, 2);
            for (int i1 = 0; i1 < sidechair1.chairseat.Count(); i1++)
                g.DrawRectangle(Pens.Yellow, sidechair1.chairseat[i1].X, sidechair1.chairseat[i1].Y, 2, 2);
        }
        private void PrintSideChair_Click(object sender, EventArgs e)
        {
            this.Refresh();// clear screen and redraw something 
            OpenFileDialog fileDlg = new OpenFileDialog();
            fileDlg.Filter = "data files (*.data)|*.data| All files (*.*)|*.*";
            if (fileDlg.ShowDialog() == DialogResult.OK)
            {
                try
                {

                    using (FileStream FS = new FileStream(fileDlg.FileName,
                                                FileMode.Open, FileAccess.Read))
                    {
                        BinaryFormatter formatter = new BinaryFormatter();
                        publicsideChair = (SideViewChair)formatter.Deserialize(FS);
                    }

                }
                catch (Exception)
                {
                    MessageBox.Show(this, "Unable to load the image file!", "Load Image Failed");
                }

            }
            // autimatically copy the select side view chair to the sidechairTamplate1 in the debug/bin derectory 
            using (FileStream fs = new FileStream("sidechairTamplat1", FileMode.Create, FileAccess.Write))
            {


                BinaryFormatter b = new BinaryFormatter();
                b.Serialize(fs, publicsideChair);// save the unistrokePt that is the resampled and reprocessing strokes points
                fs.Close();

            }// save template to file
            PrintSdChair(publicsideChair);
            
        }

        private void PlSitOnSdCh_Click(object sender, EventArgs e)
        {
            PlsitOnSdChair();
        }

        //savetable
        private void button4_Click(object sender, EventArgs e)
        {
            Renderer renderer = new Renderer();
            System.Drawing.Graphics g = this.CreateGraphics();
            Stroke[] tablestrk = new Stroke[m_InkOverlay.Selection.Count];
            int counts = m_InkOverlay.Selection.Count;
            for (int i = 0; i < m_InkOverlay.Selection.Count; i++)
            {
                tablestrk[i] = m_InkOverlay.Selection[i];

            }
            Array.Sort(tablestrk, (x, y) => (x.GetBoundingBox().Top + x.GetBoundingBox().Bottom).CompareTo(y.GetBoundingBox().Top + y.GetBoundingBox().Bottom));// sort according to the bounding box's center point's y from large to small 
            Stroke leg1, leg2, surface; 
            if (tablestrk[counts - 2].GetBoundingBox().Left < tablestrk[counts - 1].GetBoundingBox().Left)
            {
               leg1 = tablestrk[counts - 2];
                leg2 = tablestrk[counts - 1];
            }
            else
            {
                leg1 = tablestrk[counts - 1];
                leg2 = tablestrk[counts - 2];
            }
            surface = tablestrk[0];
              List<Point> leg1Pt = ResamPlePt(renderer, g, leg1.GetPoints(), leg1.GetPoints().Count() / 2);
                    List<Point> leg2Pt = ResamPlePt(renderer, g, leg2.GetPoints(), leg2.GetPoints().Count() / 2);

                    List<Point> surfacePt = ResamPlePt(renderer, g, surface.GetPoints(), surface.GetPoints().Count() / 2);
                    PublicTable = new table(leg1Pt, leg2Pt, surfacePt);

            
            SaveFileDialog savedlg = new SaveFileDialog();

            savedlg.Filter = "data files (*.data)|*.data| All files (*.*)|*.*";
            if (savedlg.ShowDialog() == DialogResult.OK)
            {
                //Stream stream = savedlg.OpenFile();
                try
                {

                    using (FileStream fs = new FileStream(savedlg.FileName, FileMode.Create, FileAccess.Write))
                    {


                        BinaryFormatter b = new BinaryFormatter();
                        b.Serialize(fs, PublicTable);// save the unistrokePt that is the resampled and reprocessing strokes points
                        fs.Close();

                    }// save template to file



                }
                catch (Exception)
                {
                    MessageBox.Show(this, "Unable to load the image file!", "save file Failed");
                }
                //stream.Close();


            }
            for (int i1 = 0; i1 < PublicTable.chairleg_1.Count(); i1++)
                g.DrawRectangle(Pens.Red, PublicTable.chairleg_1[i1].X, PublicTable.chairleg_1[i1].Y, 2, 2);
            for (int i1 = 0; i1 < PublicTable.chairleg_2.Count(); i1++)
                g.DrawRectangle(Pens.Green, PublicTable.chairleg_2[i1].X, PublicTable.chairleg_2[i1].Y, 2, 2);
            for (int i1 = 0; i1 < PublicTable.surface.Count(); i1++)
                g.DrawRectangle(Pens.Blue, PublicTable.surface[i1].X, PublicTable.surface[i1].Y, 2, 2);
            using (FileStream fs1 = new FileStream("TableTemplate.data", FileMode.Create, FileAccess.Write))
            {


                BinaryFormatter b1 = new BinaryFormatter();
                b1.Serialize(fs1, PublicTable);// save the unistrokePt that is the resampled and reprocessing strokes points
                fs1.Close();

            }// save template to file
            Point[] tableSurface = PublicTable.surface.ToArray();

            // table surface's leftmost point and right most point: 
            Array.Sort(tableSurface, (x, y) => (x.X).CompareTo(y.X));
            Point RightMostPt = tableSurface[tableSurface.Count() - 1];
            Point LeftMostPt = tableSurface[0];
            g.DrawRectangle(Pens.Red, RightMostPt.X, RightMostPt.Y, 2, 2);
            g.DrawRectangle(Pens.Yellow, LeftMostPt.X, LeftMostPt.Y, 2, 2);
        
        }

        private void ChatingScene_Click(object sender, EventArgs e)
        {
            System.Drawing.Graphics g = this.CreateGraphics();
            this.Refresh(); // clear the form for redraw something
            SideViewChair newSdchairLeft = new SideViewChair(); // left chair 
            SideViewChair newSdchairRight = new SideViewChair(); // right chair 
            peopleMd newplmd1 = new peopleMd();
            peopleMd newplmd2 = new peopleMd();
            table newtable1= new table();
            double d = 1;
            // left chair 
                  using (FileStream fs1 = new FileStream("A:/pen based project/complex_sideviewChair/Left_chair1.data", FileMode.Open))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                  public_complex_SdvChair = (complextSdvChair)formatter.Deserialize(fs1);
                      // rescale and translate 
                  public_complex_SdvChair.points = ScaleDimToListPt(public_complex_SdvChair.points, 220, 0.3, Boundingbox_sdvchair(public_complex_SdvChair.frame));
                  public_complex_SdvChair.frame = rescale_sdvChair(public_complex_SdvChair.frame);
                  
                  newSdchairLeft = public_complex_SdvChair.frame;
                

            }
                 Point  LeftOriginPt = new Point((Boundingbox_sdvchair(public_complex_SdvChair.frame).Left + Boundingbox_sdvchair(public_complex_SdvChair.frame).Right) / 2, (Boundingbox_sdvchair(public_complex_SdvChair.frame).Top + Boundingbox_sdvchair(public_complex_SdvChair.frame).Bottom) / 2);
                  Point offset_chair1 = new Point();
                  Point centerpoint_chair1 = CentroidPoint(public_complex_SdvChair.points.ToArray());

                  offset_chair1.X = LeftOriginPt.X - centerpoint_chair1.X;
                  offset_chair1.Y = LeftOriginPt.Y - centerpoint_chair1.Y;



            // right chair
                  complextSdvChair complex_chair_right = new complextSdvChair();
                  complex_chair_right.frame = new SideViewChair();
                  complex_chair_right.points = new List<Point>();
                  using (FileStream fs2 = new FileStream("A:/pen based project/complex_sideviewChair/Right_chair1.data", FileMode.Open))
            {
                
                BinaryFormatter formatter = new BinaryFormatter();
                complex_chair_right = (complextSdvChair)formatter.Deserialize(fs2);
                newSdchairRight = complex_chair_right.frame;
               // rescale 
                complex_chair_right.frame = rescale_sdvChair(complex_chair_right.frame);
                complex_chair_right.points = ScaleDimToListPt(complex_chair_right.points, 220, 0.3, Boundingbox_sdvchair(newSdchairRight));
                newSdchairRight = complex_chair_right.frame;
                  

                  }
                  Point RightOriginPt = new Point((Boundingbox_sdvchair(complex_chair_right.frame).Left + Boundingbox_sdvchair(complex_chair_right.frame).Right) / 2, (Boundingbox_sdvchair(complex_chair_right.frame).Top + Boundingbox_sdvchair(complex_chair_right.frame).Bottom) / 2);
                  Point offset_chair2 = new Point();
                  Point centerpoint_chair2 = CentroidPoint(complex_chair_right.points.ToArray());

                  offset_chair2.X = RightOriginPt.X - centerpoint_chair2.X;
                  offset_chair2.Y = RightOriginPt.Y - centerpoint_chair2.Y;
            
            // left people 
                  ComplexHumanModel complexpeople_left = new ComplexHumanModel();
                  complexpeople_left.frame = new peopleMd();
                  complexpeople_left.points = new List<Point>();
                  using (FileStream fs3 = new FileStream("A:/pen based project/complex_human_model/Left_people_model.data", FileMode.Open))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                complexpeople_left = (ComplexHumanModel)formatter.Deserialize(fs3);
                
                      // rescale 
                complexpeople_left.points = ScaleDimToListPt(complexpeople_left.points, 260, 0.3, boundingbox_plmd(complexpeople_left.frame));
                complexpeople_left.frame = rescale_plmd(complexpeople_left.frame);
                newplmd1 = complexpeople_left.frame;
               



                      
            }
                  Point OriginPt_leftPeople = new Point((boundingbox_plmd(complexpeople_left.frame).Left + boundingbox_plmd(complexpeople_left.frame).Right )/ 2, (boundingbox_plmd(complexpeople_left.frame).Top + boundingbox_plmd(complexpeople_left.frame).Bottom) / 2);
                  Point offset_people1= new Point();
                  Point centerpoint_people1 = CentroidPoint(complexpeople_left.points.ToArray());

                  offset_people1.X = OriginPt_leftPeople.X - centerpoint_people1.X;
                  offset_people1.Y = OriginPt_leftPeople.Y - centerpoint_people1.Y;

        
            // right people
                  ComplexHumanModel complexpeople_right = new ComplexHumanModel();
                  complexpeople_right.frame = new peopleMd();
                  complexpeople_right.points = new List<Point>();
                  using (FileStream fs4 = new FileStream("A:/pen based project/complex_human_model/Right_people_model.data", FileMode.Open))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                complexpeople_right = (ComplexHumanModel)formatter.Deserialize(fs4);
                // rescale 

                complexpeople_right.points = ScaleDimToListPt(complexpeople_right.points, 260, 0.3, boundingbox_plmd(complexpeople_right.frame));
                complexpeople_right.frame = rescale_plmd(complexpeople_right.frame);

                newplmd2 = complexpeople_right.frame;
            }

                  Point OriginPt_rightPeople = new Point((boundingbox_plmd(complexpeople_right.frame).Left + boundingbox_plmd(complexpeople_right.frame).Right) / 2, (boundingbox_plmd(complexpeople_right.frame).Top + boundingbox_plmd(complexpeople_right.frame).Bottom) / 2);
                  Point offset_people2 = new Point();
                  Point centerpoint_people2 = CentroidPoint(complexpeople_right.points.ToArray());

                  offset_people2.X = OriginPt_rightPeople.X - centerpoint_people2.X;
                  offset_people2.Y = OriginPt_rightPeople.Y - centerpoint_people2.Y;

              // table  
                  using (FileStream fs5 = new FileStream("A:/pen based project/complex_table/Table_1.data", FileMode.Open))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                public_complex_Table = (complexTable)formatter.Deserialize(fs5);
                public_complex_Table.points = ScaleDimToListPt(public_complex_Table.points, 300, 0.3, boundboxofTable(public_complex_Table.frame));
                public_complex_Table.frame = rescale_table(public_complex_Table.frame);
                newtable1 = public_complex_Table.frame;

                      //scale and translate the complex table 
                 // scale the points array first, because the frame will be used to compute the boundingbox 
               
                
                // move the complex points so that it align with the frame will
                Point originPoint= new Point((boundboxofTable(newtable1).Left+ boundboxofTable(newtable1).Right)/2,(boundboxofTable(newtable1).Top+boundboxofTable(newtable1).Bottom)/2);
                Point offset = new Point();
                Point centerpoint_table = CentroidPoint(public_complex_Table.points.ToArray());
                offset.X = originPoint.X - centerpoint_table.X;
                offset.Y = originPoint.Y - centerpoint_table.Y;

                      newtable1 = MoveTable(newtable1, originPoint, new Point(631, 268));
                public_complex_Table.points = translateToPtlist(public_complex_Table.points, CentroidPoint(public_complex_Table.points.ToArray()), new Point(631-offset.X, 268-offset.Y));
                public_complex_Table.frame = newtable1;
                
            }
            Point[] tableSurface = newtable1.surface.ToArray();

            // table surface's leftmost point and right most point: 
            Array.Sort(tableSurface , (x, y) => (x.X).CompareTo(y.X));
            Point RightMostPt = tableSurface[tableSurface.Count() - 1];
            Point LeftMostPt = tableSurface[0];
            g.DrawRectangle(Pens.Red,RightMostPt.X , RightMostPt.Y, 2, 2);
            g.DrawRectangle(Pens.Yellow, LeftMostPt.X, LeftMostPt.Y, 2, 2);


            //using X coordinates of the rightmost point and the left most point of the table surface as X of reference point
            //using Y coordinates of the center point of right leg and the left leg the table surface as Y of reference point
            LeftMostPt.Y = (CentroidPoint(newtable1.chairleg_1.ToArray()).Y + LeftMostPt.Y)/2;
            RightMostPt.Y = (CentroidPoint(newtable1.chairleg_2.ToArray()).Y + RightMostPt.Y)/2;

            
          // compute the reference points for Left_sideview chair1 and Right_sideview chair2 respectively  


            Point LsdchRefPt= new Point();
            Point RsdchRefPt= new Point();     
            Point[] LChseat= newSdchairLeft.chairseat.ToArray();// left chair's seat 
            Point[] RChseat= newSdchairRight.chairseat.ToArray();// right Chair's seat  
           Array.Sort( LChseat, (x, y) => (x.X).CompareTo(y.X));
          Array.Sort( RChseat, (x, y) => (x.X).CompareTo(y.X));
          LsdchRefPt = LChseat[LChseat.Count() - 1]; // for the left chair, the edge of its chair seat should be the right most point
         RsdchRefPt=RChseat[0];// for the right chair the edge of its chair seat should be the left most point
        
        //store public chair to another variable
         complextSdvChair complex_chair_left = new complextSdvChair();
         complex_chair_left.frame = new SideViewChair();
         complex_chair_left.points = new List<Point>();
         complex_chair_left.points = public_complex_SdvChair.points;
        // move left chair and right chair and move people to sit on it 
         newSdchairLeft = MoveSdChair(newSdchairLeft, LsdchRefPt, LeftMostPt);
         complex_chair_left.frame = newSdchairLeft;
         // move the complex left chair points to align the new left chair 
        Point newleftOriginPt= new Point((Boundingbox_sdvchair(newSdchairLeft).Left +Boundingbox_sdvchair(newSdchairLeft).Right)/2-offset_chair1.X,(Boundingbox_sdvchair(newSdchairLeft).Top+Boundingbox_sdvchair(newSdchairLeft).Bottom)/2-offset_chair1.Y);
        
            complex_chair_left.points = translateToPtlist(complex_chair_left.points, CentroidPoint(complex_chair_left.points.ToArray()), newleftOriginPt);

            // align the complex chair points to the new chair 
         newSdchairRight = MoveSdChair(newSdchairRight, RsdchRefPt, RightMostPt);
         complex_chair_right.frame = newSdchairRight;
         Point newRightOriginPt = new Point((Boundingbox_sdvchair(newSdchairRight).Left + Boundingbox_sdvchair(newSdchairRight).Right) / 2-offset_chair2.X, (Boundingbox_sdvchair(newSdchairRight).Top + Boundingbox_sdvchair(newSdchairRight).Bottom) / 2-offset_chair2.Y);
        
      complex_chair_right.points = translateToPtlist(complex_chair_right.points, CentroidPoint(complex_chair_right.points.ToArray()), newRightOriginPt);
            
            // move the people model 
            newplmd1 = movePltoSdchair(newplmd1, newSdchairLeft);
            complexpeople_left.frame = newplmd1;
            Point newLeftOriginPt1 = new Point((boundingbox_plmd(complexpeople_left.frame).Left + boundingbox_plmd(complexpeople_left.frame).Right )/ 2-offset_people1.X, (boundingbox_plmd(complexpeople_left.frame).Top + boundingbox_plmd(complexpeople_left.frame).Bottom) / 2-offset_people1.Y);
            complexpeople_left.points = translateToPtlist(complexpeople_left.points, CentroidPoint(complexpeople_left.points.ToArray()), newLeftOriginPt1);
       // right people 
            newplmd2 = movePltoSdchair(newplmd2, newSdchairRight);
            complexpeople_right.frame = newplmd2;
            Point newRightOriginPt1 = new Point((boundingbox_plmd(complexpeople_right.frame).Left + boundingbox_plmd(complexpeople_right.frame).Right) / 2-offset_people2.X, (boundingbox_plmd(complexpeople_right.frame).Top + boundingbox_plmd(complexpeople_right.frame).Bottom) / 2-offset_people2.Y);

            complexpeople_right.points = translateToPtlist(complexpeople_right.points, CentroidPoint(complexpeople_right.points.ToArray()), newRightOriginPt1);
         
           

       
            
            // print the scene of peoplemd sit on the chair on the form 
       /*
            PrintSdChair(newSdchairLeft);
         PrintSdChair(newSdchairRight);
         printPlmd(newplmd1);
         printPlmd(newplmd2);
         PrintTable(newtable1);
                                 */
    // draw the complex object representation  
           //draw the complex left chair
         for (int i1 = 0; i1 < complex_chair_left.points.Count(); i1++)
             g.DrawRectangle(Pens.Red, complex_chair_left.points[i1].X, complex_chair_left.points[i1].Y, 2, 2);
           // draw the complex right chair
         for (int i1 = 0; i1 < complex_chair_right.points.Count(); i1++)
             g.DrawRectangle(Pens.Red, complex_chair_right.points[i1].X, complex_chair_right.points[i1].Y, 2, 2);
         // draw people model
         for (int i1 = 0; i1 < complexpeople_left.points.Count(); i1++)
             g.DrawRectangle(Pens.Red, complexpeople_left.points[i1].X, complexpeople_left.points[i1].Y, 2, 2);
         for (int i1 = 0; i1 < complexpeople_right.points.Count(); i1++)
             g.DrawRectangle(Pens.Red, complexpeople_right.points[i1].X, complexpeople_right.points[i1].Y, 2, 2);
            // draw table 
         for (int i1 = 0; i1 < public_complex_Table.points.Count(); i1++)
             g.DrawRectangle(Pens.Red, public_complex_Table.points[i1].X, public_complex_Table.points[i1].Y, 2, 2);

        }
         
        // referPt2 is orignal point  
        private SideViewChair MoveSdChair( SideViewChair SdChair, Point referPt1, Point referPt2)
        {
             List<Point> newChBk = translateToPtlist(SdChair.chairback, referPt1, referPt2);
            List<Point> newChSeat = translateToPtlist(SdChair.chairseat, referPt1, referPt2);
            List<Point> newChleg1 = translateToPtlist(SdChair.chairleg_1, referPt1, referPt2);
            List<Point> newChleg2 = translateToPtlist(SdChair.chairleg_2, referPt1, referPt2);
            
            SideViewChair newfrntch = new SideViewChair(newChleg1,newChleg2,newChBk,newChSeat);
            return  newfrntch;
        }
        private FrontChair MovefrntChair(FrontChair SdChair, Point referPt1, Point referPt2)
        {
            List<Point> newChBk = translateToPtlist(SdChair.chairback, referPt1, referPt2);
            List<Point> newChSeat = translateToPtlist(SdChair.chairseat, referPt1, referPt2);
            List<Point> newChleg1 = translateToPtlist(SdChair.chairleg_1, referPt1, referPt2);
            List<Point> newChleg2 = translateToPtlist(SdChair.chairleg_2, referPt1, referPt2);
            List<Point> newchShaft = translateToPtlist(SdChair.chairshaft, referPt1, referPt2);
            FrontChair newfrntch = new FrontChair(newchShaft, newChleg1, newChleg2, newChBk, newChSeat);
            return newfrntch;
        }
        // referPt2 is original point 
        private peopleMd MovePlmd(peopleMd plmd, Point referPt1, Point referPt2)
        {
            List<Point> back = translateToPtlist(plmd.back, referPt1, referPt2);
            List<Point> hand1 = translateToPtlist(plmd.hand_1, referPt1, referPt2);
            List<Point> hand2 = translateToPtlist(plmd.hand_2, referPt1, referPt2);
            List<Point> plHead = translateToPtlist(plmd.head, referPt1, referPt2);
            List<Point> leg1 = translateToPtlist(plmd.leg_1, referPt1, referPt2);
            List<Point> leg2 = translateToPtlist(plmd.leg_2, referPt1, referPt2);
            peopleMd newplmd = new peopleMd(leg1, leg2, back, hand1, hand2, plHead);
            return newplmd; 
        }

        private table MoveTable(table tb, Point referPt1, Point referPt2)
        {
            List<Point> surface = translateToPtlist(tb.surface, referPt1, referPt2);
                 List<Point> tbleg1= translateToPtlist(tb.chairleg_1,referPt1,referPt2);
                 List<Point> tbleg2 = translateToPtlist(tb.chairleg_2, referPt1, referPt2);
                 table newTb = new table(tbleg1, tbleg2, surface);
                 return newTb;
        }



        private peopleMd movePltoSdchair(peopleMd plmd, SideViewChair SdCh)
        {
            Point referPt1 = CentroidPoint(SdCh.chairseat.ToArray());// center point of the the chair cross shaft;
            Point referPt2 = new Point();
            Point[] backpt = plmd.back.ToArray();
            Array.Sort(backpt, (x, y) => (x.Y).CompareTo(y.Y));// sort in decent order;
            referPt2 = backpt[(backpt.Count() - 1)]; // take the point in the lowest point of the back as reference point; 
            // move the plmd so that the chair will be sitted by the plmd

            List<Point> back = translateToPtlist(plmd.back, referPt2, referPt1);
            List<Point> hand1 = translateToPtlist(plmd.hand_1, referPt2, referPt1);
            List<Point> hand2 = translateToPtlist(plmd.hand_2, referPt2, referPt1);
            List<Point> plHead = translateToPtlist(plmd.head, referPt2, referPt1);
            List<Point> leg1 = translateToPtlist(plmd.leg_1, referPt2, referPt1);
            List<Point> leg2 = translateToPtlist(plmd.leg_2, referPt2, referPt1);
            peopleMd newplmd = new peopleMd(leg1, leg2, back, hand1, hand2, plHead);
            return newplmd; 
            
        }

        private void PrintTable(table tab1)
        {
            System.Drawing.Graphics g = this.CreateGraphics();
            for (int i1 = 0; i1 < tab1.chairleg_1.Count(); i1++)
                g.DrawRectangle(Pens.Red, tab1.chairleg_1[i1].X, tab1.chairleg_1[i1].Y, 2, 2);
            for (int i1 = 0; i1 < tab1.chairleg_2.Count(); i1++)
                g.DrawRectangle(Pens.Green, tab1.chairleg_2[i1].X, tab1.chairleg_2[i1].Y, 2, 2);
            for (int i1 = 0; i1 < tab1.surface.Count(); i1++)
                g.DrawRectangle(Pens.Blue, tab1.surface[i1].X, tab1.surface[i1].Y, 2, 2);
        }

        private void addRtPeople(int n)
        {
           peopleMd newplmd= new peopleMd();
            using (FileStream fs = new FileStream("RightPlmd.data", FileMode.Open))
            {
                BinaryFormatter formatter = new BinaryFormatter();
               newplmd = (peopleMd)formatter.Deserialize(fs);
            }
            for (int i = 0; i < n; i++)
            {
                
                publicScene.PeopleGroup.Add(newplmd);
            }
            
        }
        private void addLtPeople(int n)
        {
            peopleMd newplmd = new peopleMd();
            using (FileStream fs = new FileStream("LeftPlmd.data", FileMode.Open))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                newplmd = (peopleMd)formatter.Deserialize(fs);
            }
            for (int i = 0; i < n; i++)
            {
               
                publicScene.PeopleGroup.Add(newplmd);
            }

        }
        private void addRtchair(int n)
        {
            SideViewChair newSdchairRight = new SideViewChair();
            using (FileStream fs2 = new FileStream("RightChair.data", FileMode.Open))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                newSdchairRight = (SideViewChair)formatter.Deserialize(fs2);
            }
            for (int i = 0; i < n; i++)
            {
                publicScene.sideViewChairs.Add(newSdchairRight);
            }
        }


            private void printScene()
        {
            publicScene.PeopleGroup = new List<peopleMd>();
            publicScene.tables = new List<table>();
            publicScene.sideViewChairs = new List<SideViewChair>();
            publicScene.frontchairs = new List<FrontChair>();
           
            addLtPeople(1);
            addRtPeople(1);
            addRtchair(1);
            for (int i = 0; i < publicScene.sideViewChairs.Count; i++)
            {
                PrintSdChair(publicScene.sideViewChairs[i]);
            }
            for ( int i = 0; i < publicScene.PeopleGroup.Count; i++)
            {
                printPlmd(publicScene.PeopleGroup[i]);
            }
            for (int i = 0; i < publicScene.frontchairs.Count; i++)
            {
                printFrCh(publicScene.frontchairs[i]);
            }
            for (int i = 0; i < publicScene.tables.Count; i++)
            {
                PrintTable(publicScene.tables[i]);
            }

        }

            private void print_publice_scene()
            {


                for (int i = 0; i < publicScene.sideViewChairs.Count; i++)
                {
                    PrintSdChair(publicScene.sideViewChairs[i]);
                }
                for (int i = 0; i < publicScene.PeopleGroup.Count; i++)
                {
                    printPlmd(publicScene.PeopleGroup[i]);
                }
                for (int i = 0; i < publicScene.frontchairs.Count; i++)
                {
                    printFrCh(publicScene.frontchairs[i]);
                }
                for (int i = 0; i < publicScene.tables.Count; i++)
                {
                    PrintTable(publicScene.tables[i]);
                }

            }

            private void LoadObjectToScene()
            {
               
            }


        private void PrintTable1_Click(object sender, EventArgs e)
        {
            printScene();
        }

        // initialize the Scene_structure 
        private void Initialize_SceneStructure()    
        {
            /* publicSceneStructure is a innstance of the SceneSturcture,  before use it, pleae allocate memory for it :
            SceneSturcture publicSceneStructure = new SceneSturcture();*/  // the object's arrangement information 
            publicSceneStructure.FrntChairsInfo = new List<frontChInfo>();
            publicSceneStructure.PeopleGrpInfo = new List<PeopleInfo>();
            publicSceneStructure.SdvChairsInfo = new List<SdvchairInfo>();
            publicSceneStructure.tablesInfo = new List<tableInfo>();
            Point centerpoint = new Point();
            
            /* front chairs infomation */ 
            frontChInfo frontchairinfo = new frontChInfo();
            frontchairinfo.centerLocation.X = 725;
            frontchairinfo.centerLocation.Y = 57;
            frontchairinfo.view = 2; // front view 
            publicSceneStructure.FrntChairsInfo.Add(frontchairinfo);

            /* side view chairs infomation */
            SdvchairInfo sideviewchair_info = new SdvchairInfo();
            sideviewchair_info.centerLocation.X = 895;
            sideviewchair_info.centerLocation.Y = 206;
            sideviewchair_info.viewside = 1;// right view 
            publicSceneStructure.SdvChairsInfo.Add(sideviewchair_info);
            /* second sideview chair information */
            sideviewchair_info.centerLocation.X = 396;
            sideviewchair_info.centerLocation.Y = 196;
            sideviewchair_info.viewside = 0;// left view 
            publicSceneStructure.SdvChairsInfo.Add(sideviewchair_info);

            /* peoplemd infomation */ 
             // right view people sit on right chair 
            PeopleInfo plmd_info = new PeopleInfo(1, 0, 0, 1, new Point(815, 179));
            publicSceneStructure.PeopleGrpInfo.Add(plmd_info);
           //front view people stant 
            plmd_info.sitingchair_index = -1;//no related chair
            plmd_info.sitOrstand = 1;
            plmd_info.typeofChair = -1;// no related chair 
            plmd_info.viewside = 2;
            plmd_info.CentLocation = new Point(446, 103);

            publicSceneStructure.PeopleGrpInfo.Add(plmd_info);
          /* front view chair */
           frontChInfo frontchInformation = new frontChInfo(2, new Point(725, 57));
           publicSceneStructure.FrntChairsInfo.Add(frontchInformation);

            /* table information */
           tableInfo table_infomation = new tableInfo(new Point(631, 168));        
           publicSceneStructure.tablesInfo.Add(table_infomation);


           using (FileStream fs = new FileStream("scene_structure.data", FileMode.Create, FileAccess.Write))
           {


               BinaryFormatter b = new BinaryFormatter();
                
               b.Serialize(fs, publicSceneStructure);// save the unistrokePt that is the resampled and reprocessing strokes points
               fs.Close();

           }

           using (FileStream fs1 = new FileStream("scene_structure.data", FileMode.Open))
           {
               BinaryFormatter formatter = new BinaryFormatter();
               publicSceneStructure = (SceneSturcture)formatter.Deserialize(fs1);
           }
        }

        private void Initialize_SceneStructure1()
        {
            /* publicSceneStructure is a innstance of the SceneSturcture,  before use it, pleae allocate memory for it :
            SceneSturcture publicSceneStructure = new SceneSturcture();*/
            // the object's arrangement information 
            publicSceneStructure.FrntChairsInfo = new List<frontChInfo>();
            publicSceneStructure.PeopleGrpInfo = new List<PeopleInfo>();
            publicSceneStructure.SdvChairsInfo = new List<SdvchairInfo>();
            publicSceneStructure.tablesInfo = new List<tableInfo>();
            Point centerpoint = new Point();

            /* front chairs infomation */
            frontChInfo frontchairinfo = new frontChInfo();
            frontchairinfo.centerLocation.X = 458;
            frontchairinfo.centerLocation.Y = 348;
            frontchairinfo.view = 2; // front view 
            publicSceneStructure.FrntChairsInfo.Add(frontchairinfo);
            frontChInfo frontchairinfo1 = new frontChInfo();
            frontchairinfo1.centerLocation.X = 300;
            frontchairinfo1.centerLocation.Y = 353;
            frontchairinfo1.view = 2; // front view 
            publicSceneStructure.FrntChairsInfo.Add(frontchairinfo1);

            /* side view chairs infomation */
            SdvchairInfo sideviewchair_info = new SdvchairInfo();
            sideviewchair_info.centerLocation.X = 667;
            sideviewchair_info.centerLocation.Y = 383;
            sideviewchair_info.viewside = 1;// right view 
            publicSceneStructure.SdvChairsInfo.Add(sideviewchair_info);
            /* second sideview chair information */
            sideviewchair_info.centerLocation.X = 241;
            sideviewchair_info.centerLocation.Y = 406;
            sideviewchair_info.viewside = 0;// left view 
            publicSceneStructure.SdvChairsInfo.Add(sideviewchair_info);
            /* third sideview chair information */
            sideviewchair_info.centerLocation.X = 141;
            sideviewchair_info.centerLocation.Y = 364;
            sideviewchair_info.viewside = 0;// left view 
            publicSceneStructure.SdvChairsInfo.Add(sideviewchair_info);

            /* peoplemd infomation */
            /*
                 // right view people sit on right chair 
                PeopleInfo plmd_info = new PeopleInfo(1, 0, 0, 1, new Point(815, 179));
                publicSceneStructure.PeopleGrpInfo.Add(plmd_info);
               //front view people stant 
                plmd_info.sitingchair_index = -1;//no related chair
                plmd_info.sitOrstand = 1;
                plmd_info.typeofChair = -1;// no related chair 
                plmd_info.viewside = 2;
                plmd_info.CentLocation = new Point(446, 103);
            */

            //publicSceneStructure.PeopleGrpInfo.Add(plmd_info);
            /* front view chair */
            //frontChInfo frontchInformation = new frontChInfo(2, new Point(725, 57));
            //publicSceneStructure.FrntChairsInfo.Add(frontchInformation);

            /* table information */
            tableInfo table_infomation = new tableInfo(new Point(387, 397));
            publicSceneStructure.tablesInfo.Add(table_infomation);


            using (FileStream fs = new FileStream("scene_structure1.data", FileMode.Create, FileAccess.Write))
            {


                BinaryFormatter b = new BinaryFormatter();

                b.Serialize(fs, publicSceneStructure);// save the unistrokePt that is the resampled and reprocessing strokes points
                fs.Close();

            }

            using (FileStream fs1 = new FileStream("scene_structure1.data", FileMode.Open))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                publicSceneStructure = (SceneSturcture)formatter.Deserialize(fs1);
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
        private void MouseCordinateMethod(object sender, MouseEventArgs e)
        {
            Point  position = e.Location;
           Point  point = PointToScreen(position);
            textBox2.Clear();
            textBox2.Text = "X: " + position.X +"\n" + "Y: " + position.Y;

        }

     /* load objects template , which will influence the output (print) */
        private void load_object_template()
        {
            /*
            SideViewChair public_LeftSdVCh = new SideViewChair(); // ok
            SideViewChair public_RightSdVCh = new SideViewChair();//ok 
            FrontChair public_LeftFrntChair = new FrontChair();
            FrontChair public_RightFrntChair = new FrontChair();
            peopleMd public_Stand_Plmd = new peopleMd();
            
            peopleMd public_Sit_RightPlmd = new peopleMd();//ok
            peopleMd public_Sit_LeftPlmd = new peopleMd();//ok
                     public_RightFrntChair         */


            /* left side chair*/
            using (FileStream fs1 = new FileStream("LeftChair.data", FileMode.Open))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                public_LeftSdVCh = (SideViewChair)formatter.Deserialize(fs1);
               public_LeftSdVCh= rescale_sdvChair(public_LeftSdVCh);

            }

            /* right side chair */
            using (FileStream fs2 = new FileStream("RightChair.data", FileMode.Open))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                public_RightSdVCh = (SideViewChair)formatter.Deserialize(fs2);
                public_RightSdVCh = rescale_sdvChair(public_RightSdVCh);
            }
            /* left sit people */
            using (FileStream fs3 = new FileStream("LeftPlmd.data", FileMode.Open))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                public_Sit_LeftPlmd = (peopleMd)formatter.Deserialize(fs3);
                public_Sit_LeftPlmd = rescale_plmd(public_Sit_LeftPlmd);
            }



            /* right sit people */
            using (FileStream fs4 = new FileStream("RightPlmd.data", FileMode.Open))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                public_Sit_RightPlmd = (peopleMd)formatter.Deserialize(fs4);
                public_Sit_RightPlmd = rescale_plmd(public_Sit_RightPlmd);
            }

            /* table*/
            using (FileStream fs5 = new FileStream("tableTemplate.data", FileMode.Open))
            {
                BinaryFormatter formatter = new BinaryFormatter();
               PublicTable = (table)formatter.Deserialize(fs5);
               PublicTable = rescale_table(PublicTable);
            }
            /* stand people model*/
            using (FileStream fs6 = new FileStream("front_view_plmd.data", FileMode.Open))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                public_Stand_Plmd = (peopleMd)formatter.Deserialize(fs6);
                public_Stand_Plmd = rescale_plmd(public_Stand_Plmd);
            }
             using (FileStream fs7 = new FileStream("frntChairTemplate.data", FileMode.Open))
              {
                  BinaryFormatter formatter = new BinaryFormatter();
                  public_RightFrntChair = (FrontChair) formatter.Deserialize(fs7);
                  public_RightFrntChair = rescale_frontChair(public_RightFrntChair);
              }
             
        }

        private Rectangle Boundingbox_sdvchair(SideViewChair sdvchair)
        {
            int max_X = 0;
            int max_Y = 0;
            int min_X = 5000;
            int min_Y=5000;

            max_X = sdvchair.chairback.Max(p => p.X);     
            min_Y = sdvchair.chairback.Min(p => p.Y);// chairback has min Y value
            min_X = sdvchair.chairback.Min(p => p.X);
            max_Y = sdvchair.chairleg_1.Max(p => p.Y);// chair leg has max Y value 
            if(max_Y> sdvchair.chairleg_2.Max(p => p.Y))
            {
                max_Y = sdvchair.chairleg_2.Max(p => p.Y);
            }
         // compare x value
            if (max_X< sdvchair.chairleg_1.Max(p=>p.X))
                max_X=sdvchair.chairleg_1.Max(p=>p.X);
            if(max_X< sdvchair.chairleg_2.Max(p=>p.X))
                max_X = sdvchair.chairleg_2.Max(p => p.X);
            if (max_X < sdvchair.chairseat.Max(p => p.X))
                max_X = sdvchair.chairseat.Max(p => p.X);
            // fid min_x value        
            if (min_X > sdvchair.chairleg_1.Min(p => p.X))
                min_X = sdvchair.chairleg_1.Min(p => p.X);
            if (min_X > sdvchair.chairleg_2.Min(p => p.X))
                min_X = sdvchair.chairleg_2.Min(p => p.X);
            if (min_X > sdvchair.chairseat.Min(p => p.X))
                min_X = sdvchair.chairseat.Min(p => p.X);

            Rectangle boundingbox = new Rectangle(min_X, min_Y, max_X - min_X, max_Y - min_Y);
            return boundingbox; 

        }

        private Rectangle boundboxofTable(table tb)
        {
            int min_X = tb.surface.Min(p => p.X);
            int max_Y = tb.chairleg_1.Max(p => p.Y);
            if (max_Y < tb.chairleg_2.Max(p => p.Y))
                max_Y = tb.chairleg_2.Max(p => p.Y);
            int max_X = tb.surface.Max(p => p.X);
            int min_Y = tb.surface.Min(p => p.Y);
            Rectangle boundingbox = new Rectangle(min_X, min_Y, max_X - min_X, max_Y - min_Y);
            return boundingbox;

        }
        private Rectangle boundingbox_plmd(peopleMd plmd)
        {
            int min_X = plmd.leg_1.Min(p => p.X);
            int max_X = plmd.leg_1.Max(p => p.X);
            int min_Y = plmd.head.Min(p => p.Y);// topese point has smallest Y value 
            int Max_Y = plmd.leg_1.Max(p => p.Y);

            if (min_X > plmd.hand_1.Min(p => p.X))
                min_X = plmd.hand_1.Min(p => p.X);
            if (min_X > plmd.hand_2.Min(p => p.X))
                min_X = plmd.hand_2.Min(p => p.X);

            if (min_X > plmd.back.Min(p => p.X))
                min_X = plmd.back.Min(p => p.X);

            if (max_X < plmd.leg_2.Max(p => p.X))
                max_X = plmd.leg_2.Max(p => p.X);
            if (max_X < plmd.hand_1.Max(p => p.X))
                max_X = plmd.hand_1.Max(p => p.X);
            if(max_X < plmd.hand_2.Max(p => p.X))
                max_X = plmd.hand_2.Max(p => p.X);

            if (max_X < plmd.back.Max(p => p.X))
                max_X = plmd.back.Max(p => p.X);

            if (Max_Y < plmd.leg_2.Max(p => p.Y))
                Max_Y = plmd.leg_2.Max(p => p.Y);
            Rectangle boundingbox = new Rectangle(min_X, min_Y, max_X - min_X, Max_Y - min_Y);
            return boundingbox;
           
        }

        private Rectangle boundboxofFrontChair(FrontChair frntch)
        {
            int max_X = 0;
            int max_Y = 0;
            int min_X = 5000;
            int min_Y = 5000;

            max_X = frntch.chairback.Max(p => p.X);
            min_Y = frntch.chairback.Min(p => p.Y);// chairback has largest Y value
            min_X = frntch.chairback.Min(p => p.X);
            max_Y = frntch.chairleg_1.Max(p => p.Y);// chair leg has min Y value 
            if (max_Y < frntch.chairleg_2.Max(p => p.Y))
            {
                max_Y = frntch.chairleg_2.Max(p => p.Y);
            }
            // compare x value
            if (max_X < frntch.chairleg_1.Max(p => p.X))
                max_X = frntch.chairleg_1.Max(p => p.X);
            if (max_X < frntch.chairleg_2.Max(p => p.X))
                max_X = frntch.chairleg_2.Max(p => p.X);
            if (max_X < frntch.chairseat.Max(p => p.X))
                max_X = frntch.chairseat.Max(p => p.X);
            // find min_x value        
            if (min_X > frntch.chairleg_1.Min(p => p.X))
                min_X = frntch.chairleg_1.Min(p => p.X);
            if (min_X > frntch.chairleg_2.Min(p => p.X))
                min_X = frntch.chairleg_2.Min(p => p.X);
            if (min_X > frntch.chairseat.Min(p => p.X))
                min_X = frntch.chairseat.Min(p => p.X);

            Rectangle boundingbox = new Rectangle(min_X, min_Y, max_X - min_X, max_Y - min_Y);
            return boundingbox; 
        }
        private SideViewChair rescale_sdvChair(SideViewChair sdvchair)
        {
            Rectangle boundingbox= Boundingbox_sdvchair(sdvchair);
            sdvchair.chairback = ScaleDimToListPt(sdvchair.chairback, 220, 0.3, boundingbox);
            sdvchair.chairseat = ScaleDimToListPt(sdvchair.chairseat, 220, 0.3, boundingbox);
            sdvchair.chairleg_1 = ScaleDimToListPt(sdvchair.chairleg_1, 220, 0.3, boundingbox);
            sdvchair.chairleg_2 = ScaleDimToListPt(sdvchair.chairleg_2, 220, 0.3, boundingbox);

          
            return sdvchair;
        }

        private FrontChair rescale_frontChair(FrontChair frontchair)
        {
            int scale_size = 220;
            Rectangle boundingbox = boundboxofFrontChair(frontchair);
            frontchair.chairback = ScaleDimToListPt(frontchair.chairback, scale_size, 0.3, boundingbox);
            frontchair.chairseat = ScaleDimToListPt(frontchair.chairseat, scale_size, 0.3, boundingbox);
            frontchair.chairleg_1 = ScaleDimToListPt(frontchair.chairleg_1, scale_size, 0.3, boundingbox);
            frontchair.chairleg_2 = ScaleDimToListPt(frontchair.chairleg_2, scale_size, 0.3, boundingbox);
            frontchair.chairshaft = ScaleDimToListPt(frontchair.chairshaft, scale_size, 0.3, boundingbox);

            return frontchair;   
        }

        private peopleMd rescale_plmd(peopleMd plmd)
        {
            int scale_size = 260;
            Rectangle boundingbox = boundingbox_plmd(plmd);
            plmd.head = ScaleDimToListPt(plmd.head, scale_size, 0.3, boundingbox);
            plmd.hand_1 = ScaleDimToListPt(plmd.hand_1, scale_size, 0.3, boundingbox);
            plmd.hand_2 = ScaleDimToListPt(plmd.hand_2, scale_size, 0.3, boundingbox);
            plmd.back = ScaleDimToListPt(plmd.back, scale_size, 0.3, boundingbox);
            plmd.leg_1 = ScaleDimToListPt(plmd.leg_1, scale_size, 0.3, boundingbox);
            plmd.leg_2 = ScaleDimToListPt(plmd.leg_2, scale_size, 0.3, boundingbox);

            return plmd;

        }

        private table rescale_table(table tb)
        {
            int scale_size = 300;
            Rectangle boundingbox = boundboxofTable(tb);
            tb.surface = ScaleDimToListPt(tb.surface, scale_size, 0.3, boundingbox);
            tb.chairleg_1 = ScaleDimToListPt(tb.chairleg_1, scale_size, 0.3, boundingbox);
            tb.chairleg_2 = ScaleDimToListPt(tb.chairleg_2, scale_size, 0.3, boundingbox);

            return tb;

        }

        // complexchair 
        private void Complex_Object_Click(object sender, EventArgs e)
        {
             // load public complex points 
            using (FileStream fs6 = new FileStream("Public_complex_Points.data", FileMode.Open))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                public_complex_ObjectPoints = (List<Point>)formatter.Deserialize(fs6);
            }

            if (m_InkOverlay.Selection.Count == 5)// frontview chair
            {
                

                // create complex_plmd
                public_complex_FrontChair.frame = new FrontChair();
                public_complex_FrontChair.points = new List<Point>();

                public_complex_FrontChair.frame = publicfrntChair;
                public_complex_FrontChair.points = public_complex_ObjectPoints;



                SaveFileDialog savedlg = new SaveFileDialog();

                savedlg.Filter = "data files (*.data)|*.data| All files (*.*)|*.*";
                if (savedlg.ShowDialog() == DialogResult.OK)
                {
                    //Stream stream = savedlg.OpenFile();
                    try
                    {

                        using (FileStream fs = new FileStream(savedlg.FileName, FileMode.Create, FileAccess.Write))
                        {


                            BinaryFormatter b = new BinaryFormatter();
                            b.Serialize(fs, public_complex_FrontChair);// save the unistrokePt that is the resampled and reprocessing strokes points
                            fs.Close();

                        }// save template to file



                    }
                    catch (Exception)
                    {
                        MessageBox.Show(this, "Unable to load the image file!", "save file Failed");
                    }
                    //stream.Close();

                }// if

                Graphics g = this.CreateGraphics();

                for (int i1 = 0; i1 < public_complex_FrontChair.points.Count(); i1++)
                    g.DrawRectangle(Pens.Red, public_complex_FrontChair.points[i1].X, public_complex_FrontChair.points[i1].Y, 2, 2);

                printFrCh(public_complex_FrontChair.frame);
            }// if the chair is front view chair 

            if (m_InkOverlay.Selection.Count == 4)// sideview chair
            {
                public_complex_SdvChair.frame = new SideViewChair();
                public_complex_SdvChair.points = new List<Point>();

                public_complex_SdvChair.frame = publicsideChair;
                public_complex_SdvChair.points = public_complex_ObjectPoints;



                SaveFileDialog savedlg = new SaveFileDialog();

                savedlg.Filter = "data files (*.data)|*.data| All files (*.*)|*.*";
                if (savedlg.ShowDialog() == DialogResult.OK)
                {
                    //Stream stream = savedlg.OpenFile();
                    try
                    {

                        using (FileStream fs = new FileStream(savedlg.FileName, FileMode.Create, FileAccess.Write))
                        {


                            BinaryFormatter b = new BinaryFormatter();
                            b.Serialize(fs, public_complex_SdvChair);// save the unistrokePt that is the resampled and reprocessing strokes points
                            fs.Close();

                        }// save template to file



                    }
                    catch (Exception)
                    {
                        MessageBox.Show(this, "Unable to load the image file!", "save file Failed");
                    }
                    //stream.Close();

                }// if

                Graphics g = this.CreateGraphics();

                for (int i1 = 0; i1 < public_complex_SdvChair.points.Count(); i1++)
                    g.DrawRectangle(Pens.Red, public_complex_SdvChair.points[i1].X, public_complex_SdvChair.points[i1].Y, 2, 2);

                PrintSdChair(public_complex_SdvChair.frame);
            }
        }

        
        // print complexed object 
        private void Print_Complex_objects_Click(object sender, EventArgs e)
        {
           
            Graphics g = this.CreateGraphics();
            List<Point> complex_object = new List<Point>();
         
          
  

            OpenFileDialog fileDlg = new OpenFileDialog();
            fileDlg.Filter = "data files (*.data)|*.data| All files (*.*)|*.*";
            if (fileDlg.ShowDialog() == DialogResult.OK)
            {
                try
                {

                    using (FileStream FS = new FileStream(fileDlg.FileName,
                                                FileMode.Open, FileAccess.Read))
                    {
                        BinaryFormatter formatter = new BinaryFormatter();
                        complex_object = (List<Point>)formatter.Deserialize(FS);
                    }

                }
                catch (Exception)
                {
                    MessageBox.Show(this, "Unable to load the image file!", "Load Image Failed");
                }

            }
            public_complex_ObjectPoints = complex_object;
            this.Refresh();
            for (int i1 = 0; i1 < complex_object.Count(); i1++)
                g.DrawRectangle(Pens.Red, complex_object[i1].X, complex_object[i1].Y, 2, 2);
           

            using (FileStream fs = new FileStream("Public_complex_Points.data", FileMode.Create, FileAccess.Write))
            {


                BinaryFormatter b = new BinaryFormatter();
                b.Serialize(fs, public_complex_ObjectPoints);// save the unistrokePt that is the resampled and reprocessing strokes points
                fs.Close();

            }// save template to file

        }

        // save table 
        private void button4_Click_1(object sender, EventArgs e)
        {
            Renderer renderer = new Renderer();
            System.Drawing.Graphics g = this.CreateGraphics();
            Stroke[] tablestrk = new Stroke[m_InkOverlay.Selection.Count];
            int counts = m_InkOverlay.Selection.Count;
            for (int i = 0; i < m_InkOverlay.Selection.Count; i++)
            {
                tablestrk[i] = m_InkOverlay.Selection[i];

            }
            Array.Sort(tablestrk, (x, y) => (x.GetBoundingBox().Top + x.GetBoundingBox().Bottom).CompareTo(y.GetBoundingBox().Top + y.GetBoundingBox().Bottom));// sort according to the bounding box's center point's y from large to small 
            Stroke leg1, leg2, surface;
            if (tablestrk[counts - 2].GetBoundingBox().Left < tablestrk[counts - 1].GetBoundingBox().Left)
            {
                leg1 = tablestrk[counts - 2];
                leg2 = tablestrk[counts - 1];
            }
            else
            {
                leg1 = tablestrk[counts - 1];
                leg2 = tablestrk[counts - 2];
            }
            surface = tablestrk[0];
            List<Point> leg1Pt = ResamPlePt(renderer, g, leg1.GetPoints(), leg1.GetPoints().Count() / 2);
            List<Point> leg2Pt = ResamPlePt(renderer, g, leg2.GetPoints(), leg2.GetPoints().Count() / 2);

            List<Point> surfacePt = ResamPlePt(renderer, g, surface.GetPoints(), surface.GetPoints().Count() / 2);
            PublicTable = new table(leg1Pt, leg2Pt, surfacePt);


            SaveFileDialog savedlg = new SaveFileDialog();

            savedlg.Filter = "data files (*.data)|*.data| All files (*.*)|*.*";
            if (savedlg.ShowDialog() == DialogResult.OK)
            {
                //Stream stream = savedlg.OpenFile();
                try
                {

                    using (FileStream fs = new FileStream(savedlg.FileName, FileMode.Create, FileAccess.Write))
                    {


                        BinaryFormatter b = new BinaryFormatter();
                        b.Serialize(fs, PublicTable);// save the unistrokePt that is the resampled and reprocessing strokes points
                        fs.Close();

                    }// save template to file



                }
                catch (Exception)
                {
                    MessageBox.Show(this, "Unable to load the image file!", "save file Failed");
                }
                //stream.Close();


            }
            for (int i1 = 0; i1 < PublicTable.chairleg_1.Count(); i1++)
                g.DrawRectangle(Pens.Red, PublicTable.chairleg_1[i1].X, PublicTable.chairleg_1[i1].Y, 2, 2);
            for (int i1 = 0; i1 < PublicTable.chairleg_2.Count(); i1++)
                g.DrawRectangle(Pens.Green, PublicTable.chairleg_2[i1].X, PublicTable.chairleg_2[i1].Y, 2, 2);
            for (int i1 = 0; i1 < PublicTable.surface.Count(); i1++)
                g.DrawRectangle(Pens.Blue, PublicTable.surface[i1].X, PublicTable.surface[i1].Y, 2, 2);
            using (FileStream fs1 = new FileStream("TableTemplate.data", FileMode.Create, FileAccess.Write))
            {


                BinaryFormatter b1 = new BinaryFormatter();
                b1.Serialize(fs1, PublicTable);// save the unistrokePt that is the resampled and reprocessing strokes points
                fs1.Close();

            }// save template to file
            Point[] tableSurface = PublicTable.surface.ToArray();

            // table surface's leftmost point and right most point: 
            Array.Sort(tableSurface, (x, y) => (x.X).CompareTo(y.X));
            Point RightMostPt = tableSurface[tableSurface.Count() - 1];
            Point LeftMostPt = tableSurface[0];
            g.DrawRectangle(Pens.Red, RightMostPt.X, RightMostPt.Y, 2, 2);
            g.DrawRectangle(Pens.Yellow, LeftMostPt.X, LeftMostPt.Y, 2, 2);
        }


        // complex people model, need to first draw the frame and then select and click the corresponding button 
        private void complex_plmd_Click(object sender, EventArgs e)
        {
            

            // load public complex points 
            using (FileStream fs6 = new FileStream("Public_complex_Points.data", FileMode.Open))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                public_complex_ObjectPoints = (List<Point>)formatter.Deserialize(fs6);
            }

            // create complex_plmd
            public_complex_humanModel.frame = new peopleMd();
            public_complex_humanModel.points = new List<Point>();
           
            public_complex_humanModel.frame = pubicPlmd;
            public_complex_humanModel.points = public_complex_ObjectPoints;



            SaveFileDialog savedlg = new SaveFileDialog();

            savedlg.Filter = "data files (*.data)|*.data| All files (*.*)|*.*";
            if (savedlg.ShowDialog() == DialogResult.OK)
            {
                //Stream stream = savedlg.OpenFile();
                try
                {

                    using (FileStream fs = new FileStream(savedlg.FileName, FileMode.Create, FileAccess.Write))
                    {


                        BinaryFormatter b = new BinaryFormatter();
                        b.Serialize(fs, public_complex_humanModel);// save the unistrokePt that is the resampled and reprocessing strokes points
                        fs.Close();

                    }// save template to file



                }
                catch (Exception)
                {
                    MessageBox.Show(this, "Unable to load the image file!", "save file Failed");
                }
                //stream.Close();

            }// if

            Graphics g = this.CreateGraphics();

            for (int i1 = 0; i1 < public_complex_humanModel.points.Count(); i1++)
                g.DrawRectangle(Pens.Red, public_complex_humanModel.points[i1].X, public_complex_humanModel.points[i1].Y, 2, 2);

            printPlmd(public_complex_humanModel.frame);

        }

        // complex table construction   
        private void complex_Table_Click(object sender, EventArgs e)
        {
            // load public complex points 
            using (FileStream fs6 = new FileStream("Public_complex_Points.data", FileMode.Open))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                public_complex_ObjectPoints = (List<Point>)formatter.Deserialize(fs6);
            }

            // create complex table 
            public_complex_Table.frame = new table();
            public_complex_Table.points = new List<Point>();

            public_complex_Table.frame = PublicTable;
            public_complex_Table.points = public_complex_ObjectPoints;



            SaveFileDialog savedlg = new SaveFileDialog();

            savedlg.Filter = "data files (*.data)|*.data| All files (*.*)|*.*";
            if (savedlg.ShowDialog() == DialogResult.OK)
            {
                //Stream stream = savedlg.OpenFile();
                try
                {

                    using (FileStream fs = new FileStream(savedlg.FileName, FileMode.Create, FileAccess.Write))
                    {


                        BinaryFormatter b = new BinaryFormatter();
                        b.Serialize(fs, public_complex_Table);// save the unistrokePt that is the resampled and reprocessing strokes points
                        fs.Close();

                    }// save template to file



                }
                catch (Exception)
                {
                    MessageBox.Show(this, "Unable to load the image file!", "save file Failed");
                }
                //stream.Close();

            }// if

            Graphics g = this.CreateGraphics();

            for (int i1 = 0; i1 < public_complex_Table.points.Count(); i1++)
                g.DrawRectangle(Pens.Red, public_complex_Table.points[i1].X, public_complex_Table.points[i1].Y, 2, 2);

            PrintTable(public_complex_Table.frame);
        }

    }

}



