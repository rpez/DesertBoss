using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;
using System;

namespace DesertMod.NPCs.Bosses
{
    public class DesertBoss : ModNPC
    {
        // AI
        private int aiPhase = 0;
        private int attackTimer = 0;

        // Animation
        private int frame = 0;
        private double counting;

        // Stats
        private int daggerDamage = 1;
        private float daggerSpeed = 25f;
        private int halberdDamage = 10;
        private float halberdSpeed = 20f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cursed Sphinx");
            Main.npcFrameCount[npc.type] = 4;
        }

        public override void SetDefaults()
        {
            npc.width = 64;
            npc.height = 64;

            npc.boss = true;
            npc.aiStyle = -1;

            npc.lifeMax = 1000;
            npc.damage = 10;
            npc.defense = 10;
            npc.knockBackResist = 0f;

            npc.noGravity = true;
            npc.noTileCollide = true;

            npc.HitSound = SoundID.NPCHit1;
            npc.DeathSound = SoundID.NPCDeath1;

            //bossBag = mod.ItemType("DesertBossTreasureBag");
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            base.ScaleExpertStats(numPlayers, bossLifeScale);
            npc.lifeMax = (int)(npc.lifeMax * bossLifeScale);
            npc.damage = (int)(npc.damage * 1.3f);
        }

        public override void AI()
        {
            // Add "tick" to the phase counter of AI
            aiPhase++;

            npc.TargetClosest(true);
            Player player = Main.player[npc.target];
            Vector2 target = npc.HasPlayerTarget ? player.Center : Main.npc[npc.target].Center;

            // NPC initial rotation
            npc.rotation = 0.0f;
            npc.netAlways = true;
            npc.TargetClosest(true);

            // Prevents overheal
            if (npc.life >= npc.lifeMax) npc.life = npc.lifeMax;

            // Handles despawning
            if (npc.target < 0 || npc.target == 255 || player.dead || !player.active)
            {
                npc.TargetClosest(false);
                npc.direction = 1;
                npc.velocity.Y -= 0.1f;
                if (npc.timeLeft > 20)
                {
                    npc.timeLeft = 20;
                    return;
                }
            }

            // Battle phases
            Vector2 bossCenter = npc.Center;
            Vector2 towardsPlayer = target - bossCenter;
            towardsPlayer.Normalize();

            // When healthy
            if (npc.life >= npc.lifeMax / 2)
            {
                // Movement
                int distance = (int)Vector2.Distance(target, npc.Center);
                MoveTowards(npc, target - towardsPlayer * 300f, (float)(distance > 300 ? 13f : 7f), 30f);
                npc.netUpdate = true;

                // Dagger attack
                if (aiPhase >= 50)
                {
                    // The actual projectile
                    Projectile.NewProjectile(bossCenter, towardsPlayer * daggerSpeed, mod.ProjectileType("DesertBossProjectileSpiritDagger"), daggerDamage, 5f);
                    
                    // Workaround ghosting trail
                    for (int i = 1; i < 5; i++)
                    {
                        Vector2 dir = towardsPlayer;
                        dir.Normalize();
                        Vector2 pos = bossCenter - dir * 15f * i;
                        Projectile.NewProjectile(pos, towardsPlayer * daggerSpeed, mod.ProjectileType("DesertBossProjectileSpiritDagger"), 0, 0f);
                    }

                    aiPhase = 0;
                }
            }

            // Below half HP
            else if (npc.life < npc.lifeMax / 2)
            {
                if (aiPhase >= 300)
                {
                    npc.velocity = Vector2.Zero;
                    //Projectile.NewProjectile(bossCenter, towardsPlayer * halberdSpeed, mod.ProjectileType("DesertBossProjectileHalberd"), halberdDamage, 5f);
                    Projectile.NewProjectile(bossCenter - new Vector2(-100f, 0f), Vector2.Zero, mod.ProjectileType("DesertBossProjectileHalberd"), halberdDamage, 5f, npc.whoAmI);
                    aiPhase = 0;
                }
            }
        }

        public override void FindFrame(int frameHeight)
        {
            if (frame == 0)
            {
                counting += 1.0;
                if (counting < 8.0)
                {
                    npc.frame.Y = 0;
                }
                else if (counting < 16.0)
                {
                    npc.frame.Y = frameHeight;
                }
                else if (counting < 24.0)
                {
                    npc.frame.Y = frameHeight * 2;
                }
                else if (counting < 32.0)
                {
                    npc.frame.Y = frameHeight * 3;
                }
                else
                {
                    counting = 0.0;
                }
            }
        }

        public override void NPCLoot()
        {
            base.NPCLoot();
        }

        public override void BossLoot(ref string name, ref int potionType)
        {
            base.BossLoot(ref name, ref potionType);
        }

        private void MoveTowards(NPC npc, Vector2 playerTarget, float speed, float turnResistance)
        {
            Vector2 move = playerTarget - npc.Center;
            float length = move.Length();
            if (length > speed)
            {
                move *= speed / length;
            }
            move = (npc.velocity * turnResistance + move) / (turnResistance + 1f);
            length = move.Length();
            if (length > speed)
            {
                move *= speed / length;
            }
            npc.velocity = move;
        }
    }
}