using PokemonGo.RocketAPI;
using System;
using Windows.Devices.Sensors;
using Windows.Foundation;

namespace PokemonGo_UWP.Utils
{
  public class CompassHelper
  {
    private Compass compass;
    private Inclinometer incline;
    private SharpDX.Quaternion quat;
    private SharpDX.Matrix3x3 mat;
    private MagnetometerAccuracy accuracy;

    private int samples = 10;
    private float[] values;
    private int index = 0;

    public CompassHelper()
    {
      values = new float[samples];
      for (int i = 0; i < samples; ++i) values[i] = 0;
      
      quat = new SharpDX.Quaternion();
      mat = new SharpDX.Matrix3x3();

      compass = Compass.GetDefault();
      if (compass != null)
      {
        compass.ReportInterval = compass.MinimumReportInterval > 200 ? compass.MinimumReportInterval : 200;
        compass.ReadingChanged += new TypedEventHandler<Compass, CompassReadingChangedEventArgs>(CompassReadingChanged);
      }

      incline = Inclinometer.GetDefault();
      if (incline != null)
      {
        incline.ReportInterval = incline.MinimumReportInterval > 20 ? incline.MinimumReportInterval : 20;
        incline.ReadingChanged += new TypedEventHandler<Inclinometer, InclinometerReadingChangedEventArgs>(InclineReadingChanged);
      }
    }

    async private void CompassReadingChanged(object sender, CompassReadingChangedEventArgs e)
    {
      accuracy = e.Reading.HeadingAccuracy;
      if (Accuracy != null) Accuracy(this, new AccuracyReading(accuracy));
    }

    bool initYaw = false;
    float Yaw = 0.0f;

    async private void InclineReadingChanged(object sender, InclinometerReadingChangedEventArgs e)
    {
      if (incline == null) return;
      var heading = e.Reading;
         
      float pitch = (180.0f - heading.PitchDegrees - 90.0f);
      float yaw = heading.YawDegrees;

      Yaw = yaw;
//      if (!initYaw) { initYaw = true; Yaw = yaw; }
//      values[index] = yaw;


//      if (++index == samples)
//      {
//        index = 0;
//
//        float sum = 0;
//        int num = 0;
//
//        // average the values - throw out any that are outliers
//        for (int i = 0; i < samples; ++i)
//        {
//          if (Math.Abs(values[i] - Yaw) > 90) continue;
//          sum += values[i];
//          ++num;
//        }
//
//        if (num > 0) Yaw = sum / num;
//      }

      mat = SharpDX.Matrix3x3.RotationYawPitchRoll(Yaw * (float)Math.PI / 180.0f, pitch * (float)Math.PI / 180.0f, 0.0f);
      quat = SharpDX.Quaternion.RotationYawPitchRoll(Yaw * (float)Math.PI / 180.0f, pitch * (float)Math.PI / 180.0f, 0.0f);
    }

    public SharpDX.Quaternion Quat { get { return quat; } }
    public SharpDX.Matrix3x3 Matrix { get { return mat; } }

    public delegate void AccuracyEvent(object sender, AccuracyReading e);
    public event AccuracyEvent Accuracy;
  }

  public class AccuracyReading : EventArgs
  {
    private readonly MagnetometerAccuracy acc;

    public AccuracyReading(MagnetometerAccuracy a)
    {
      acc = a;
    }

    public MagnetometerAccuracy Accuracy
    {
      get { return acc; }
    }
  }
}
