using SharpDX;
using LeagueSharp;
using LeagueSharp.Common;
using System.Linq;
using System.Collections.Generic;
using System;

namespace Lord_s_Vayne.Condemn
{
    class Lords2
    {

        public static void Run()
        {
            var target = TargetSelector.GetTarget(Program.zzrot.Range, TargetSelector.DamageType.Physical);
            var position = target.Position;
            var pointwaswall = false;
        var d = target.Position.Distance(Efinishpos(target));
                for (var i = 0; i<d; i += 10)
                {
                    var dist = i > d ? d : i;
        var point = target.Position.Extend(Efinishpos(target), dist);
                    if (pointwaswall)
                    {
                        if (point.IsWall())
                        {
                        Program.E.CastOnUnit(target);
                        }
                    }
                    if (point.IsWall())
                    {
                        pointwaswall = true;
                    }
                }
            }            
private static Vector3 Efinishpos(Obj_AI_Hero ts)
        {
            return (Vector3)ObjectManager.Player.Position.Extend(ts.Position, ObjectManager.Player.Distance(ts.Position) + 490).To2D();
        }
    }
    }

