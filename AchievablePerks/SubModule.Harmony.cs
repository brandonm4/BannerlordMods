using AchievablePerks.Diagnostics;
using AchievablePerks.Patches;
using HarmonyLib;
using ModLib;
using System;
using System.Collections.Generic;
using System.Reflection;

using TaleWorlds.Core;
using TaleWorlds.Library;

namespace AchievablePerks
{
    public partial class AchievablePerksSubModule
    {
        internal static readonly Harmony Harmony = new Harmony(nameof(AchievablePerks));
        public static IDictionary<Type, IPatch> ActivePatches = new Dictionary<Type, IPatch>();

        #region HarmoryPatches

        private static void ApplyPatches(Game game)
        {
            //ActivePatches.Clear();

            foreach (var patch in Patches)
            {
                try
                {
                    patch.Reset();
                }
                catch (Exception ex)
                {
                    //Error(ex, $"Error while resetting patch: {patch.GetType().Name}");
                    //MessageBox.Show("TournamentXP Patch Error", $"Error while applying patch: {patch.GetType().Name}\n" + ex.ToStringFull());
                    ErrorLog.Log($"Error while resetting patch: {patch.GetType().Name}");
                    ErrorLog.Log(ex.ToStringFull());
                }

                try
                {
                    if (patch.IsApplicable(game))
                    {
                        try
                        {
                            patch.Apply(game);
                        }
                        catch (Exception ex)
                        {
                            //  Error(ex, $"Error while applying patch: {patch.GetType().Name}");
                            ErrorLog.Log($"Error while applying patch: {patch.GetType().Name}");
                            ErrorLog.Log(ex.ToStringFull());
                        }
                    }
                }
                catch (Exception ex)
                {
                    ErrorLog.Log($"Error while checking if patch is applicable: {patch.GetType().Name}");
                    ErrorLog.Log(ex.ToStringFull());
                }

                var patchApplied = patch.Applied;
                if (patchApplied)
                    ActivePatches[patch.GetType()] = patch;
                if (AchievablePerksSettings.Instance.DebugMode)
                    ShowMessage($"{(patchApplied ? "Applied" : "Skipped")} Patch: {patch.GetType().Name}", (patchApplied ? Colors.Cyan : Colors.Red));
            }
        }

        private static LinkedList<IPatch> _patches;

        private static LinkedList<IPatch> Patches
        {
            get
            {
                if (_patches != null)
                    return _patches;

                var patchInterfaceType = typeof(IPatch);
                _patches = new LinkedList<IPatch>();

                foreach (var type in typeof(AchievablePerksSubModule).Assembly.GetTypes())
                {
                    if (type.IsInterface || type.IsAbstract)
                        continue;
                    if (!patchInterfaceType.IsAssignableFrom(type))
                        continue;

                    try
                    {
                        var patch = (IPatch)Activator.CreateInstance(type, true);
                        //var patch = (IPatch) FormatterServices.GetUninitializedObject(type);
                        _patches.AddLast(patch);
                    }
                    catch (TargetInvocationException tie)
                    {
                        //     Error(tie.InnerException, $"Failed to create instance of patch: {type.FullName}");
                    }
                    catch (Exception ex)
                    {
                        // Error(ex, $"Failed to create instance of patch: {type.FullName}");
                    }
                }

                return _patches;
            }
        }

        #endregion HarmoryPatches
    }
}

/* Harmony Patch code courtesty of Bannerlord Community Patch*/
/* License for this code file */
/*MIT License

Copyright (c) 2020 Tyler Young

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/
