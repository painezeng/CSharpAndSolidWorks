using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace ComparePart
{
    public class Compare
    {
        public SldWorks swApp = null;

        public static SldWorks thisSldworks;

        public Compare(SldWorks sldworks)
        {
            thisSldworks = sldworks;
        }

        public void setColour(Color c)
        {
            SldWorks swApp = ConnectToSolidWorks();

            Component2 swComp = default(Component2);

            ModelDoc2 swModel = (ModelDoc2)swApp.ActiveDoc;

            SelectionMgr swSelMgr = (SelectionMgr)swModel.SelectionManager;

            swComp = swSelMgr.GetSelectedObjectsComponent3(1, 0);

            var vMatProp = (double[])swComp.MaterialPropertyValues;// GetModelMaterialPropertyValues("");

            if (vMatProp == null)
            {
                ModelDoc2 swCompModel = (ModelDoc2)swComp.GetModelDoc();

                vMatProp = (double[])swCompModel.MaterialPropertyValues;
            }

            vMatProp[0] = c.R / 255;
            vMatProp[1] = c.G / 255;
            vMatProp[2] = c.B / 255;

            swComp.MaterialPropertyValues = vMatProp;
        }

        private SldWorks ConnectToSolidWorks()
        {
            return thisSldworks;
        }

        /// <summary>
        /// 当结果返回True 时 表示两个零件不一样，结果false时表示 两个零件一样
        /// </summary>
        /// <param name="sendTocustomer"></param>
        /// <param name="localModel"></param>
        /// <returns></returns>
        public bool CheckTwoParts(string sendTocustomer, string localModel)
        {
            bool different1 = false;

            bool different2 = false;

            swApp = ConnectToSolidWorks();
            // swApp = ConnectToSolidWorks();

            //加载参考关系零件
            swApp.SetUserPreferenceIntegerValue((int)swUserPreferenceIntegerValue_e.swLoadExternalReferences, (int)swLoadExternalReferences_e.swLoadExternalReferences_ChangedOnly);

            //后台模式 前台不显示界面

            swApp.EnableBackgroundProcessing = true;

            //禁止记录文件路径
            swApp.SetUserPreferenceToggle((int)swUserPreferenceToggle_e.swLockRecentDocumentsList, true);

            swApp.OpenDoc(localModel, 1);

            swApp.OpenDoc(sendTocustomer, 1);

            ModelDoc2 swModel = (ModelDoc2)swApp.ActiveDoc;

            string LocalPath = System.IO.Directory.GetParent(swModel.GetPathName()).ToString();

            string tempAssembly = swApp.GetUserPreferenceStringValue((int)swUserPreferenceStringValue_e.swDefaultTemplateAssembly);

            AssemblyDoc assemblyDoc = (AssemblyDoc)swApp.NewDocument(tempAssembly, 0, 0, 0);

            Component2 insertComponentSendtoCustomer = assemblyDoc.AddComponent5(sendTocustomer, 0, "", false, "", 0, 0, 0);
            Component2 insertComponentLocal = assemblyDoc.AddComponent5(localModel, 0, "", false, "", 0, 0, 0);

            string sendSelect = insertComponentSendtoCustomer.GetSelectByIDString();
            string localSelect = insertComponentLocal.GetSelectByIDString();
            swModel = (ModelDoc2)swApp.ActiveDoc;

            bool b1 = swModel.Extension.SelectByID2("Point1@Origin" + "@" + sendSelect, "EXTSKETCHPOINT", 0, 0, 0, false, 0, null, 0);
            bool b2 = swModel.Extension.SelectByID2("Point1@Origin" + "@" + localSelect, "EXTSKETCHPOINT", 0, 0, 0, true, 0, null, 0);
            int longstatus;

            Mate2 mate2 = assemblyDoc.AddMate5(20, -1, false, 0, 0.001, 0.001, 0.001, 0.001, 0, 0, 0, false, false, 0, out longstatus);

            swModel = (ModelDoc2)swApp.ActiveDoc;

            swModel.SaveAs(LocalPath + @"\TopCheck.sldasm");

            //不显示特征树
            //swModel.Extension.HideFeatureManager(true);

            swModel.ClearSelection2(true);
            //swModel.FeatureManager.ViewFeatures = false;

            //FeatureManager featureManager = swModel.FeatureManager;

            //禁用特征树
            //featureManager.EnableFeatureTree = false;

            // swModel.FeatureManager.EnableFeatureTreeWindow = false;

            #region first join

            Component2 sendToCustomerBodies = default(Component2);
            List<Component2> sendToCustomerBodiesList = new List<Component2>();

            object swFaceOrPlane = default(object);

            assemblyDoc.InsertNewVirtualPart(swFaceOrPlane, out sendToCustomerBodies);

            sendToCustomerBodies.Select(true);

            assemblyDoc.FixComponent();
            sendToCustomerBodies.Select(true);
            assemblyDoc.EditPart();

            insertComponentSendtoCustomer.Select(false);
            insertComponentLocal.Select(true);

            assemblyDoc.InsertJoin2(false, false);

            swModel = (ModelDoc2)swApp.ActiveDoc;
            swModel.BreakAllExternalReferences();

            object[] splits = sendToCustomerBodies.Name2.Split('^');
            // string compName = System.IO.Directory.GetParent(swModel.GetPathName()) + "\\" + splits[0];
            string compName = System.IO.Directory.GetParent(swModel.GetPathName()) + "\\" + "localBodies-1";

            ModelDoc2 compModel = default(ModelDoc2);
            compModel = (ModelDoc2)sendToCustomerBodies.GetModelDoc();

            if (compModel.GetType() == (int)swDocumentTypes_e.swDocPART)
            {
                compName = compName + ".sldprt";
            }
            else
            {
                compName = compName + ".sldasm";
            }

            bool ret;

            ret = sendToCustomerBodies.SaveVirtualComponent(compName);

            sendToCustomerBodiesList.Add(sendToCustomerBodies);

            insertComponentSendtoCustomer.Select(false);

            swModel = (ModelDoc2)sendToCustomerBodies.GetModelDoc2();

            #region 获取所有零件中的零件，每一个实体做一次反切

            List<string> bodyNamesCustomer = new List<string>();

            PartDoc swPart = null;
            object vBody;
            swPart = (PartDoc)swModel;
            // Solid bodies
            object[] vBodyArr = null;
            Body2 swBody = default(Body2);
            vBodyArr = (object[])swPart.GetBodies2((int)swBodyType_e.swSolidBody, true);

            if ((vBodyArr != null))
            {
                // Debug.Print("  Number of solid bodies: " + vBodyArr.Length);

                foreach (object vBody_loopVariable in vBodyArr)
                {
                    vBody = vBody_loopVariable;
                    swBody = (Body2)vBody;

                    string[] vConfigName = null;
                    vConfigName = (string[])swModel.GetConfigurationNames();
                    string sMatDB = "";
                    string sMatName = swBody.GetMaterialPropertyName("", out sMatDB);

                    //bRet = swBody.RemoveMaterialProperty((int)swInConfigurationOpts_e.swAllConfiguration, (vConfigName));

                    //Debug.Print("Body--> " + swBody.Name + " " + "");

                    bodyNamesCustomer.Add(swBody.Name);
                }
            }

            //如果实体数量大于1，则继续新建对应数量的join 实体。

            OnlyKeepNamedBody(bodyNamesCustomer[0], sendToCustomerBodies.GetSelectByIDString(), assemblyDoc, sendToCustomerBodies, insertComponentSendtoCustomer, ref different1);

            if (bodyNamesCustomer.Count > 1)
            {
                assemblyDoc.EditAssembly();

                for (int i = 0; i < bodyNamesCustomer.Count - 1; i++)
                {
                    Component2 returnpart = default(Component2);
                    var partSelect = CreateNewJoinPart(assemblyDoc, insertComponentSendtoCustomer, insertComponentLocal, swModel, "localBodies-" + (i + 2), out returnpart);
                    sendToCustomerBodiesList.Add(returnpart);

                    OnlyKeepNamedBody(bodyNamesCustomer[i + 1], partSelect, assemblyDoc, returnpart, insertComponentSendtoCustomer, ref different1);
                }
            }
            // boolstatus = Part.Extension.SelectByID2("Join1[2]@localBodies-1@TopCheck", "SOLIDBODY", 0, 0, 0, True, 0, Nothing, 0)
            //Dim myFeature As Object
            //Set myFeature = Part.FeatureManager.InsertDeleteBody2(True)

            #endregion 获取所有零件中的零件，每一个实体做一次反切

            //assemblyDoc.InsertCavity4(0, 0, 0, true, 1, -1);
            Feature theFeature;

            //theFeature = swModel.FeatureByPositionReverse(0);

            //if (theFeature.Name.Contains("Cavity"))
            //{
            //    //theFeature.Select(true);

            //    swModel = (ModelDoc2)swApp.ActiveDoc;

            //    bool b = swModel.Extension.SelectByID2(theFeature.Name + "@" + sendToCustomerBodies.GetSelectByIDString(), "BODYFEATURE", 0, 0, 0, false, 0, null, 0);

            //    swModel.BreakAllExternalReferences();
            //    different1 = true;
            //    //JoinPart1 留下的: 发给客户的没有此部分。 而本地零件中有
            //}
            //else
            //{
            //    //无法Join时表示 全切了。
            //    swModel = (ModelDoc2)swApp.ActiveDoc;

            //    bool b = swModel.Extension.SelectByID2(theFeature.Name + "@" + sendToCustomerBodies.GetSelectByIDString(), "BODYFEATURE", 0, 0, 0, false, 0, null, 0);

            //    swModel.EditSuppress();
            //}
            //sendToCustomerBodies.Select(true);

            assemblyDoc.EditAssembly();

            #endregion first join

            #region sercond join2

            Component2 localBodies = default(Component2);
            List<Component2> LocalBodiesList = new List<Component2>();
            assemblyDoc.InsertNewVirtualPart(swFaceOrPlane, out localBodies);

            localBodies.Select(true);

            assemblyDoc.FixComponent();
            localBodies.Select(true);
            assemblyDoc.EditPart();

            insertComponentSendtoCustomer.Select(false);
            insertComponentLocal.Select(true);

            assemblyDoc.InsertJoin2(false, false);

            swModel = (ModelDoc2)swApp.ActiveDoc;
            swModel.BreakAllExternalReferences();

            splits = localBodies.Name2.Split('^');
            // string compName = System.IO.Directory.GetParent(swModel.GetPathName()) + "\\" + splits[0];
            compName = System.IO.Directory.GetParent(swModel.GetPathName()) + "\\" + "sendToCustomerBodies-1";

            compModel = default(ModelDoc2);
            compModel = (ModelDoc2)localBodies.GetModelDoc();

            if (compModel.GetType() == (int)swDocumentTypes_e.swDocPART)
            {
                compName = compName + ".sldprt";
            }
            else
            {
                compName = compName + ".sldasm";
            }

            ret = localBodies.SaveVirtualComponent(compName);
            LocalBodiesList.Add(localBodies);
            insertComponentLocal.Select(false);

            swModel = (ModelDoc2)localBodies.GetModelDoc2();

            #region 获取所有零件中的零件，每一个实体做一次反切

            List<string> bodyNamesLocal = new List<string>();

            PartDoc swPart2 = null;
            object vBody2;
            swPart2 = (PartDoc)swModel;
            // Solid bodies
            object[] vBodyArr2 = null;
            Body2 swBody2 = default(Body2);
            vBodyArr2 = (object[])swPart2.GetBodies2((int)swBodyType_e.swSolidBody, true);

            if ((vBodyArr2 != null))
            {
                // Debug.Print("  Number of solid bodies: " + vBodyArr.Length);

                foreach (object vBody_loopVariable in vBodyArr2)
                {
                    vBody2 = vBody_loopVariable;
                    swBody2 = (Body2)vBody2;

                    string[] vConfigName = null;
                    vConfigName = (string[])swModel.GetConfigurationNames();
                    string sMatDB = "";
                    string sMatName = swBody2.GetMaterialPropertyName("", out sMatDB);

                    //bRet = swBody.RemoveMaterialProperty((int)swInConfigurationOpts_e.swAllConfiguration, (vConfigName));

                    bodyNamesLocal.Add(swBody2.Name);
                }
            }

            //如果实体数量大于1，则继续新建对应数量的join 实体。

            OnlyKeepNamedBody(bodyNamesLocal[0], localBodies.GetSelectByIDString(), assemblyDoc, localBodies, insertComponentLocal, ref different2);

            if (bodyNamesLocal.Count > 1)
            {
                assemblyDoc.EditAssembly();

                for (int i = 0; i < bodyNamesLocal.Count - 1; i++)
                {
                    Component2 returnpart = default(Component2);
                    var partSelect = CreateNewJoinPart(assemblyDoc, insertComponentSendtoCustomer, insertComponentLocal, swModel, "sendToCustomerBodies-" + (i + 2), out returnpart);
                    LocalBodiesList.Add(returnpart);
                    OnlyKeepNamedBody(bodyNamesLocal[i + 1], partSelect, assemblyDoc, returnpart, insertComponentLocal, ref different2);
                }
            }
            // boolstatus = Part.Extension.SelectByID2("Join1[2]@localBodies-1@TopCheck", "SOLIDBODY", 0, 0, 0, True, 0, Nothing, 0)
            //Dim myFeature As Object
            //Set myFeature = Part.FeatureManager.InsertDeleteBody2(True)

            #endregion 获取所有零件中的零件，每一个实体做一次反切

            //assemblyDoc.InsertCavity4(0, 0, 0, true, 1, -1);

            //theFeature = swModel.FeatureByPositionReverse(0);

            //swModel = (ModelDoc2)localBodies.GetModelDoc2();

            //theFeature = swModel.FeatureByPositionReverse(0);

            //if (theFeature.Name.Contains("Cavity"))
            //{
            //    //theFeature.Select(true);

            //    swModel = (ModelDoc2)swApp.ActiveDoc;

            //    bool b = swModel.Extension.SelectByID2(theFeature.Name + "@" + localBodies.GetSelectByIDString(), "BODYFEATURE", 0, 0, 0, false, 0, null, 0);

            //    swModel.BreakAllExternalReferences();
            //    different2 = true;
            //    //JoinPart2 留下的: 发给客户的有此部分。 而本地零件中没有
            //}
            //else
            //{
            //    //无法Join时表示 全切了。
            //    swModel = (ModelDoc2)swApp.ActiveDoc;

            //    bool b = swModel.Extension.SelectByID2(theFeature.Name + "@" + sendToCustomerBodies.GetSelectByIDString(), "BODYFEATURE", 0, 0, 0, false, 0, null, 0);

            //    swModel.EditSuppress();
            //}

            //localBodies.Select(true);

            //assemblyDoc.EditAssembly();

            #endregion sercond join2

            #region joinPartPublic

            Component2 publicBodies = default(Component2);

            assemblyDoc.InsertNewVirtualPart(swFaceOrPlane, out publicBodies);

            publicBodies.Select(true);

            assemblyDoc.FixComponent();
            publicBodies.Select(true);
            assemblyDoc.EditPart();

            insertComponentSendtoCustomer.Select(false);
            insertComponentLocal.Select(true);

            assemblyDoc.InsertJoin2(false, false);

            swModel = (ModelDoc2)swApp.ActiveDoc;
            swModel.BreakAllExternalReferences();

            splits = publicBodies.Name2.Split('^');
            // string compName = System.IO.Directory.GetParent(swModel.GetPathName()) + "\\" + splits[0];
            compName = System.IO.Directory.GetParent(swModel.GetPathName()) + "\\" + "publicBodies";

            compModel = default(ModelDoc2);
            compModel = (ModelDoc2)publicBodies.GetModelDoc();

            if (compModel.GetType() == (int)swDocumentTypes_e.swDocPART)
            {
                compName = compName + ".sldprt";
            }
            else
            {
                compName = compName + ".sldasm";
            }

            ret = publicBodies.SaveVirtualComponent(compName);

            swModel.ClearSelection();

            foreach (var item in sendToCustomerBodiesList)
            {
                item.Select(false);

                assemblyDoc.InsertCavity4(0, 0, 0, true, 1, -1);

                theFeature = (Feature)swModel.FeatureByPositionReverse(0);

                swModel = (ModelDoc2)publicBodies.GetModelDoc2();
                theFeature = (Feature)swModel.FeatureByPositionReverse(0);

                if (theFeature.Name.Contains("Cavity"))
                {
                    //theFeature.Select(true);

                    swModel = (ModelDoc2)swApp.ActiveDoc;

                    bool b = swModel.Extension.SelectByID2(theFeature.Name + "@" + publicBodies.GetSelectByIDString(), "BODYFEATURE", 0, 0, 0, false, 0, null, 0);

                    swModel.BreakAllExternalReferences();

                    //joinPartPublic 留下的: 发给客户的有此部分。 而本地零件中没有
                }
                else
                {
                }
            }
            foreach (var item in LocalBodiesList)
            {
                item.Select(false);

                assemblyDoc.InsertCavity4(0, 0, 0, true, 1, -1);

                theFeature = (Feature)swModel.FeatureByPositionReverse(0);

                swModel = (ModelDoc2)publicBodies.GetModelDoc2();
                theFeature = (Feature)swModel.FeatureByPositionReverse(0);

                if (theFeature.Name.Contains("Cavity"))
                {
                    //theFeature.Select(true);

                    swModel = (ModelDoc2)swApp.ActiveDoc;

                    bool b = swModel.Extension.SelectByID2(theFeature.Name + "@" + publicBodies.GetSelectByIDString(), "BODYFEATURE", 0, 0, 0, false, 0, null, 0);

                    swModel.BreakAllExternalReferences();

                    //joinPartPublic 留下的: 发给客户的有此部分。 而本地零件中没有
                }
                else
                {
                }
            }

            publicBodies.Select(true);

            assemblyDoc.EditAssembly();

            #endregion joinPartPublic

            foreach (var item in sendToCustomerBodiesList)
            {
                item.Select(false);
                setColour(Color.Red);
            }
            foreach (var item in LocalBodiesList)
            {
                item.Select(false);
                setColour(Color.Blue);
            }

            swModel = (ModelDoc2)swApp.ActiveDoc;
            swModel.ClearSelection2(true);

            insertComponentLocal.Select(false);
            insertComponentSendtoCustomer.Select(true);
            swModel.HideComponent2();
            swModel.ClearSelection2(true);

            publicBodies.Select(false);
            assemblyDoc.SetComponentTransparent(true);
            setColour(Color.Green);
            swModel.EditRebuild3();
            swModel.Save();

            swModel.SaveAs3(LocalPath + @"\01_CheckResult.sldprt", 0, 0);

            swApp.CloseDoc("TopCheck.sldasm");
            swApp.CloseAllDocuments(true);
            swApp.OpenDoc(LocalPath + @"\01_CheckResult.sldprt", 1);
            swModel = (ModelDoc2)swApp.ActiveDoc;
            swModel.ShowNamedView2("*Isometric", 7);

            swModel.ViewZoomtofit2();

            swApp.SetUserPreferenceToggle((int)swUserPreferenceToggle_e.swLockRecentDocumentsList, false);
            try
            {
                System.IO.File.Delete(LocalPath + @"\TopCheck.sldasm");
                System.IO.File.Delete(LocalPath + @"\localBodies.sldprt");
                System.IO.File.Delete(LocalPath + @"\sendToCustomerBodies.sldprt");
                System.IO.File.Delete(LocalPath + @"\publicBodies.sldprt");
            }
            catch (Exception)
            {
            }

            //第二次反向剪切

            swApp.CloseDoc(sendTocustomer);
            swApp.CloseDoc(localModel);

            swModel.FeatureManager.EnableFeatureTree = true;

            swApp.SetUserPreferenceIntegerValue((int)swUserPreferenceIntegerValue_e.swLoadExternalReferences, 2);

            if (different1 == false && different2 == false)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private void OnlyKeepNamedBody(string v, string s, AssemblyDoc assemblyDoc, Component2 sendToCustomerBodies, Component2 insertComponentSendtoCustomer, ref bool diff1)
        {
            //var tempswModel = (ModelDoc2)swModel;
            var swModel = (ModelDoc2)swApp.ActiveDoc;
            swModel.Extension.SelectByID2(v + "@" + s, "SOLIDBODY", 0, 0, 0, false, 0, null, 0);

            var myFeature = swModel.FeatureManager.InsertDeleteBody2(true);

            insertComponentSendtoCustomer.Select(false);

            assemblyDoc.InsertCavity4(0, 0, 0, true, 1, -1);
            Feature theFeature;

            var tempPart = (ModelDoc2)sendToCustomerBodies.GetModelDoc2();
            theFeature = (Feature)tempPart.FeatureByPositionReverse(0);

            if (theFeature.Name.Contains("Cavity"))
            {
                //theFeature.Select(true);

                swModel = (ModelDoc2)swApp.ActiveDoc;

                bool b = swModel.Extension.SelectByID2(theFeature.Name + "@" + sendToCustomerBodies.GetSelectByIDString(), "BODYFEATURE", 0, 0, 0, false, 0, null, 0);

                swModel.BreakAllExternalReferences();
                diff1 = true;
                //JoinPart1 留下的: 发给客户的没有此部分。 而本地零件中有
            }
            else
            {
                //无法Join时表示 全切了。
                swModel = (ModelDoc2)swApp.ActiveDoc;

                bool b = swModel.Extension.SelectByID2(theFeature.Name + "@" + sendToCustomerBodies.GetSelectByIDString(), "BODYFEATURE", 0, 0, 0, false, 0, null, 0);

                swModel.EditSuppress();

                theFeature = (Feature)tempPart.FeatureByPositionReverse(1);
                if (theFeature.Name.Contains("Join"))
                {
                    b = swModel.Extension.SelectByID2(theFeature.Name + "@" + sendToCustomerBodies.GetSelectByIDString(), "BODYFEATURE", 0, 0, 0, false, 0, null, 0);

                    swModel.EditSuppress();
                }
            }
            sendToCustomerBodies.Select(true);

            assemblyDoc.EditAssembly();
        }

        public string CreateNewJoinPart(AssemblyDoc assemblyDoc, Component2 insertComponentSendtoCustomer, Component2 insertComponentLocal, ModelDoc2 swModel, string partname, out Component2 returnpart)
        {
            Component2 sendToCustomerBodies = default(Component2);

            object swFaceOrPlane = default(object);

            assemblyDoc.InsertNewVirtualPart(swFaceOrPlane, out sendToCustomerBodies);

            sendToCustomerBodies.Select(true);

            assemblyDoc.FixComponent();
            sendToCustomerBodies.Select(true);
            assemblyDoc.EditPart();

            insertComponentSendtoCustomer.Select(false);
            insertComponentLocal.Select(true);

            assemblyDoc.InsertJoin2(false, false);

            swModel = (ModelDoc2)swApp.ActiveDoc;
            swModel.BreakAllExternalReferences();

            object[] splits = sendToCustomerBodies.Name2.Split('^');
            // string compName = System.IO.Directory.GetParent(swModel.GetPathName()) + "\\" + splits[0];
            string compName = System.IO.Directory.GetParent(swModel.GetPathName()) + "\\" + partname;

            ModelDoc2 compModel = default(ModelDoc2);
            compModel = (ModelDoc2)sendToCustomerBodies.GetModelDoc();

            if (compModel.GetType() == (int)swDocumentTypes_e.swDocPART)
            {
                compName = compName + ".sldprt";
            }
            else
            {
                compName = compName + ".sldasm";
            }

            bool ret;

            ret = sendToCustomerBodies.SaveVirtualComponent(compName);

            returnpart = sendToCustomerBodies;

            return sendToCustomerBodies.GetSelectByIDString();
        }
    }
}