using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swpublished;

namespace PaineTool.NewFeature
{
    [ComVisible(true)]
    public class NewFeatureBase //: ISwComFeature
    {
        //private NewFeaturePMPage mFeaturePmPage;

        //public NewFeatureBase(NewFeaturePMPage mpFeaturePmPage)
        //{
        //    mFeaturePmPage = mpFeaturePmPage;
        //}

        //public NewFeatureBase()
        //{
        //}

        //Object ISwComFeature.Edit(object app, object modelDoc, object feature)
        //{
        //    var f = (Feature)feature;
        //    MacroFeatureData featData = (MacroFeatureData)f.GetDefinition();

        //    mFeaturePmPage.Show(featData, modelDoc);

        //    return true;
        //}

        //Object ISwComFeature.Regenerate(object app, object modelDoc, object feature)
        //{
        //    return true;
        //}

        //Object ISwComFeature.Security(object app, object modelDoc, object feature)
        //{
        //    return true;
        //}
    }
}