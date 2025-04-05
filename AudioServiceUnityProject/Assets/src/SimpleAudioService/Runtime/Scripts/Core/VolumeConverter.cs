using UnityEngine;

namespace AudioService.Core
{
    public static class VolumeConverter
    {
        /// <summary>
        /// ボリュームからデシベルへ変換（0f ~ 1f => -80db ~ 0dbへ） 
        /// </summary>
        public static float ToDecibel(float volume)
        {
            if (volume <= 0.001f)
                return -80f;
            return Mathf.Clamp(Mathf.Log10(volume) * 20f, -80f, 0f);
        }
        
        /// <summary>
        /// デシベルからボリュームへ変換（-80db ~ 0db => 0f ~ 1fへ）
        /// </summary>
        public static float ToVolume(float decibel)
        {
            if (decibel <= -80f)
                return 0f;
            return Mathf.Clamp(Mathf.Pow(10f, decibel / 20f), 0f, 1f);
        }
        
        /// <summary>
        /// 0 ~ 100の範囲の値をデシベルに変換します。
        /// </summary>
        /// <returns></returns>
        public static float ToDecibelFromPercent(float percent)
        {
            return ToDecibel(percent / 100f);
        }
        
        /// <summary>
        /// デシベルを0 ~ 100の範囲の値に変換します。
        /// </summary>
        /// <param name="decibel"></param>
        /// <returns></returns>
        public static int ToPercentFromDecibel(float decibel)
        {
            return Mathf.RoundToInt(ToVolume(decibel) * 100f);
        }
        
        /// <summary>
        /// ミュートかどうかを判定します。
        /// </summary>
        public static bool IsMuted(float decibel)
        {
            return Mathf.Approximately(decibel, -80f);
        }
    }
}