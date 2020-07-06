﻿using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;
using System;

namespace DesertMod.NPCs.Boss
{
    class GlyphPetrifying : Glyph
    {
        private float rotationAroundBoss = 0;
        private float distanceFromCenter = 300;
        private float rotationSpeed = 1;

        private int ray = -1;
        private bool shootRay = true;
        private bool rayActive = false;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Petrifying Glyph");
            Main.npcFrameCount[npc.type] = 1;
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            base.ScaleExpertStats(numPlayers, bossLifeScale);
        }

        public override void AI()
        {
            // Run base AI and if not active do not execute glyph specific AI
            base.AI();
            if (!isActive)
            {
                if (ray >= 0)
                {
                    Main.projectile[ray].timeLeft = 0;
                    ray = -1;
                }
                return;
            }

            npc.TargetClosest(true);

            double rad = rotationAroundBoss * (Math.PI / 180);

            npc.position.X = boss.Center.X - (int)(Math.Cos(rad) * distanceFromCenter) - npc.width / 2;
            npc.position.Y = boss.Center.Y - (int)(Math.Sin(rad) * distanceFromCenter) - npc.height / 2;

            // Shoot ray if it is not active
            if (!rayActive)
            {
                Player player = Main.player[npc.target];
                if (shootRay)
                {
                    ray = Projectile.NewProjectile(npc.Center, Vector2.Zero, mod.ProjectileType("DesertBossProjectileFreezeRay"), 0, 0f);
                    Main.projectile[ray].ai[0] = npc.whoAmI;
                    Main.projectile[ray].ai[1] = player.whoAmI;
                    
                    shootRay = false;
                    rayActive = true;
                }
            }

            // Kill ray when glyph is killed
            if (rayActive && npc.life <= 0)
            {
                Main.projectile[ray].timeLeft = 0;
            }

            rotationAroundBoss += rotationSpeed;
            aiPhase++;
        }

        public override void FindFrame(int frameHeight)
        {
            base.FindFrame(frameHeight);
        }
    }
}
