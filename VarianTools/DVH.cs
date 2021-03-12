using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;

namespace VarianTools
{
  public static partial class DVH
  {


    public static double VolAtDose(PlanningItem plan_item, Structure s, DoseValue d, VolumePresentation vol_pres)
    {

      // If Planning Item is Plan
      if (plan_item is PlanSetup)
      {
        double volume = ((PlanSetup)plan_item).GetVolumeAtDose(s, d, vol_pres);

        if (Double.IsNaN(volume))
          return VolAtDoseDVH(plan_item, s, d, vol_pres);
        else
          return volume;
      }

      // If Planning Item is Plan Sum
      else
        return VolAtDoseDVH(plan_item, s, d, vol_pres);
    }

    /// <summary>
    /// returns the volume in ?cc? receiving dose d (or more) for structure s INTERPOLATED FROM DVH DATA
    /// </summary>
    /// <param name="plan_item"></param>
    /// <param name="s"></param>
    /// <param name="dose"></param>
    /// <param name="vol_pres"></param>
    /// <returns></returns>
    public static double VolAtDoseDVH(PlanningItem plan_item, Structure s, DoseValue d, VolumePresentation vol_pres)
    {
      
      double binWidth = 0.1;
      DVHData dvh = plan_item.GetDVHCumulativeData(s, DoseValuePresentation.Absolute, vol_pres, binWidth);
      if (dvh == null)
        return Double.NaN;

      DVHPoint[] hist = dvh.CurveData;
      int index = (int)(hist.Length * d.Dose / dvh.MaxDose.Dose);
      if (index < 0 )
        return double.NaN;
      if (index > hist.Length)
        return 0.0;
      else
        return hist[index].Volume;
    }

    public static DoseValue StructureMaxDose(PlanningItem p, Structure s)
    {
      double binWidth = 0.1;
      DVHData dvh = p.GetDVHCumulativeData(s, DoseValuePresentation.Absolute, VolumePresentation.AbsoluteCm3, binWidth);
      if (dvh != null)
        return dvh.MaxDose;
      else
        return DoseValue.Undefined;
    }


    /// <summary>
    /// returns min dose recieved for a given volume (volume) 
    /// </summary>
    /// <param name="plan_item"></param>
    /// <param name="s"></param>
    /// <param name="volume"></param>
    /// <param name="vol_pres"></param>
    /// <returns></returns>
    public static DoseValue DoseAtVolDVH(PlanningItem plan_item, Structure s, double volume, VolumePresentation vol_pres)
    {
      // get dvh data
      double binWidth = 0.1;
      DVHData dvh = plan_item.GetDVHCumulativeData(s, DoseValuePresentation.Absolute, vol_pres, binWidth);

      if (dvh == null || dvh.CurveData.Count() == 0)
        return DoseValue.UndefinedDose();
      double absVolume = dvh.CurveData[0].VolumeUnit == "%" ? volume * dvh.Volume * 0.01 : volume;
      if (volume < 0.0 || absVolume > dvh.Volume)
        return DoseValue.UndefinedDose();

      DVHPoint[] hist = dvh.CurveData;

      for (int i = 0; i < hist.Length; i++)
      {
        if (hist[i].Volume <= volume)
          return hist[i].DoseValue;
      }

      return DoseValue.UndefinedDose();

    }

 
    /*public static DoseValue DoseAtVol(string structure, double volume, VolumePresentation vol_pres)
    {

      // get structure object
      Structure structure_obj = GetStructure(this.SelectedStructureSet, structure);

      // If Planning Item is Plan
      if (SelectedPlanningItem is PlanSetup)
      {
        DoseValue dose = ((PlanSetup)SelectedPlanningItem).GetDoseAtVolume(structure_obj, volume, vol_pres, DoseValuePresentation.Absolute);
        if (Double.IsNaN(dose.Dose))
        {
          // get dvh data

          //int i = Convert.ToInt32(volume / bin);
          MessageBox.Show("Warning NaN Detected\n" + structure + " sampling coverage is likely subobtimal\nValue will be interpolated from DVH data", "Plan Eval", MessageBoxButton.OK, MessageBoxImage.Warning);

          DVHData dvh = ((PlanSetup)SelectedPlanningItem).GetDVHCumulativeData(structure_obj, DoseValuePresentation.Absolute, vol_pres, bin);

          if (dvh == null || dvh.CurveData.Count() == 0)
            return DoseValue.UndefinedDose();
          double absVolume = dvh.CurveData[0].VolumeUnit == "%" ? volume * dvh.Volume * 0.01 : volume;
          if (volume < 0.0 || absVolume > dvh.Volume)
            return DoseValue.UndefinedDose();

          DVHPoint[] hist = dvh.CurveData;
          //return hist[i].DoseValue;

          for (int i = 0; i < hist.Length; i++)
          {
            if (hist[i].Volume <= volume)
              return hist[i].DoseValue;
          }
          return DoseValue.UndefinedDose();
        }
        else
          return dose;

      }

      // If Planning Item is Plan Sum
      else
      {
        // get dvh data

        //int i = Convert.ToInt32(volume / bin);

        DVHData dvh = ((PlanSum)SelectedPlanningItem).GetDVHCumulativeData(structure_obj, DoseValuePresentation.Absolute, vol_pres, bin);

        if (dvh == null || dvh.CurveData.Count() == 0)
          return DoseValue.UndefinedDose();
        double absVolume = dvh.CurveData[0].VolumeUnit == "%" ? volume * dvh.Volume * 0.01 : volume;
        if (volume < 0.0 || absVolume > dvh.Volume)
          return DoseValue.UndefinedDose();

        DVHPoint[] hist = dvh.CurveData;
        //return hist[i].DoseValue;



        for (int i = 0; i < hist.Length; i++)
        {
          if (hist[i].Volume <= volume)
            return hist[i].DoseValue;
        }
        return DoseValue.UndefinedDose();

      }
    }
    */




  }
}
