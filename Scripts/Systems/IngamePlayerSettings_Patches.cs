using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using HarmonyLib;

namespace MikesTweaks.Scripts.Systems
{
    [HarmonyPatch(typeof(IngamePlayerSettings))]
    public class IngamePlayerSettings_Patches
    {
        [HarmonyPatch(nameof(IngamePlayerSettings.RebindKey))]
        [HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> AllowMouseBinding(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> instructionsToList = new List<CodeInstruction>(instructions);

            for (int i = 0; i < instructionsToList.Count; i++)
            {
                var instruction = instructionsToList[i];
                if (instruction.opcode != OpCodes.Ldstr)
                    continue;
                if ((string)instruction.operand != "Mouse")
                    continue;

                instructionsToList.RemoveAt(i + 1);
                instructionsToList.RemoveAt(i);
            }

            return instructionsToList.AsEnumerable();
        }
    }
}
