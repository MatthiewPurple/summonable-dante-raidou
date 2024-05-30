using MelonLoader;
using HarmonyLib;
using Il2Cpp;
using summonable_dante_raidou;

[assembly: MelonInfo(typeof(SummonableDanteRaidou), "Summonable Dante/Raidou", "1.0.0", "Matthiew Purple")]
[assembly: MelonGame("アトラス", "smt3hd")]

namespace summonable_dante_raidou;
public class SummonableDanteRaidou : MelonMod
{
    public static short currentRecord;

    // After replacing localized text
    [HarmonyPatch(typeof(frFont), nameof(frFont.frReplaceLocalizeText))]
    private class Patch
    {
        public static void Postfix(ref string __result)
        {
            dds3GlobalWork.DDS3_GBWK.maka = 999999;
            // Simplify Mido's text when summoning Dante/Raidou
            if (__result.Contains("<SP7><FO1>It will cost <CO4>0 Macca. Are you okay with that?"))
            {
                __result = "<SP7><FO1>Are you okay with that?";
            }
            else if (__result.Contains("<SP7><FO1>Even I cannot deal with this man...")) {
                // Stock full
                if (datCalc.datCheckStockFull() != 0)
                {
                    __result = "<SP7><FO1>Your party is full.";
                }
                // Stock not full but still unable to get it --> Already in the party
                else
                {
                    __result = "<SP7><FO1>He's already with you.";
                }
            }
        }
    }

    // Before and after confirming a summon from compendium
    [HarmonyPatch(typeof(fclEncyc), nameof(fclEncyc.PrepSummon))]
    private class Patch2
    {
        public static void Prefix(ref fclEncyc.readmainwork_tag pwork)
        {
            // Remember the compendium record's ID (for another function later)
            currentRecord = pwork.recindex;
        }

        public static void Postfix(ref fclEncyc.readmainwork_tag pwork, ref int __result)
        {
            // Get the unit about to be summoned
            var pelem = dds3GlobalWork.DDS3_GBWK.encyc_record.pelem[pwork.recindex];

            // If it's Dante/Raidou (or Raidou/Dante if the Separate Raidou and Dante mod is installed)
            if (pelem.id == 192 || pelem.id == 191)
            {
                // If stock not full and there isn't already one in the party (and something else idk)
                if (datCalc.datCheckStockFull() == 0 && datCalc.datSearchDevilStock(pelem.id) == -1)
                {
                    // Allow the summon
                    __result = 1;
                }
            }          
        }
    }

    // Before starting a script
    [HarmonyPatch(typeof(fclMisc), nameof(fclMisc.fclScriptStart))]
    private class Patch3
    {
        public static void Prefix(ref int StartNo)
        {
            if (StartNo == 18)
            {
                // Get the unit about to be summoned
                var pelem = dds3GlobalWork.DDS3_GBWK.encyc_record.pelem[currentRecord];

                // If it's Dante/Raidou (or Raidou/Dante if the Separate Raidou and Dante mod is installed)
                if (pelem.id == 192 || pelem.id == 191)
                {
                    // If stock not full and there isn't already one in the party
                    if (datCalc.datCheckStockFull() == 0 && datCalc.datSearchDevilStock(pelem.id) == -1)
                    {
                        StartNo = 17;
                    }
                }
            }
        }
    }
}
