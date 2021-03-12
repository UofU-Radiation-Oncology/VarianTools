using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;

namespace VarianTools
{
  public static partial class Beams
  {

    public static JawSetBack GetJawSetbacks(ControlPoint cp, Aperture a)
    {
      var jsb = new JawSetBack();

      // get x1 setback
      List<double> lpSetBacks = new List<double>();
      foreach (var lp in a.LeafPairs)
        lpSetBacks.Add(lp.x1 - cp.JawPositions.X1);
      jsb.x1 = lpSetBacks.Min();

      // get x2 setback
      lpSetBacks.Clear();
      foreach (var lp in a.LeafPairs)
        lpSetBacks.Add(cp.JawPositions.X2 - lp.x2);
      jsb.x2 = lpSetBacks.Min();

      // get y1 setback 
      var lpY1 = a.LeafPairs.First();
      jsb.y1 = GetLeafEdgeY1(cp.Beam.MLC, lpY1.index) - cp.JawPositions.Y1;

      // get y2 setback 
      var lpY2 = a.LeafPairs.Last();
      jsb.y2 = cp.JawPositions.Y2 - GetLeafEdgeY2(cp.Beam.MLC, lpY2.index);

      return jsb;

    }

    public static double GetLeafEdgeY1(MLC mlc, int i)
    {
      double Y1edge = -110.0;
      for (int j = 0; j < i; j++)
        Y1edge += GetLeafWidth(mlc, j);
      return Y1edge;
    }

    public static double GetLeafEdgeY2(MLC mlc, int i)
    {
      double Y2edge = -105.0;
      for (int j = 0; j < i; j++)
        Y2edge += GetLeafWidth(mlc, j+1);
      return Y2edge;
    }

    public static double GetLeafWidth(MLC mlc, int i)
    {
      if (mlc.Model == "Varian High Definition 120")
      {
        if (i > 13 && i < 46)
          return 2.5;
        else
          return 5.0;
      }
      else return double.NaN;
    }


    /// <summary>
    /// returns a set of aperture objects which contain contiguous leaf pairs that are open beyond threshold t
    /// </summary>
    /// <param name="cp"></param>
    /// <param name="threshold"></param>
    /// <returns></returns>
    public static List<Aperture> GetCPApertures(ControlPoint cp, double t)
    {
      List<Aperture> Apertures = new List<Aperture>();
      bool aSearch = false; // indicates wheter an aperture is actively being searched and added to 
      Aperture a = new Aperture();
      for (int i = 0; i < cp.LeafPositions.GetLength(1); i++)
      {
        var lp = new LeafPair(i, (double)cp.LeafPositions[0, i], (double)cp.LeafPositions[1, i], t);
        // if previous aperture found
        if (!aSearch)
        {
          if (lp.isOpen())
          {
            // create new aperture
            a.LeafPairs.Add(lp);
            aSearch = true;
          }
        }
        else
        {
          if (lp.isOpen())
          {
            if (lp.isContigousWith(a.LeafPairs.Last()))
            {
              a.LeafPairs.Add(lp);
            }
            else
            {
              Apertures.Add(a);
              a = new Aperture();
              a.LeafPairs.Add(lp);
            }
          }
          else
          {
            Apertures.Add(a);
            a = new Aperture();
            aSearch = false;
          }
        }
      }
      return Apertures;
    }





  }

  public struct JawSetBack
  {
    public double x1;
    public double x2;
    public double y1;
    public double y2;
  }
  public class Aperture
  {
    public List<LeafPair> LeafPairs;
    public Aperture()
    {
      LeafPairs = new List<LeafPair>();
    }
  
  }

  public class LeafPair
  {
    public LeafPair(int i, double X1, double X2, double t)
    {
      index = i;
      x1 = X1;
      x2 = X2;
      threshold = t;
    }
    public int index;
    public double x1;
    public double x2;
    public double threshold;
    public bool isOpen()
    {
      if (Math.Abs(x1 - x2) > threshold)
        return true;
      else
        return false;
    }

    public bool isContigousWith(LeafPair plp)
    {
      if (plp.x1 < this.x2 && plp.x2 > this.x1 && plp.isOpen())
        return true;
      else
        return false;
    }
    

  }
}
