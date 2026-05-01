//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.Linq;
//using System.Text;
//using UnityEngine;

//namespace Coop
//{
//    internal class LevelMotor
//    {
//        public void Init()
//        {

//            On.LevelPlayerMotor.BufferInputs += BufferInputs;
//            On.LevelPlayerMotor.HandleJumping += HandleJumping;
//            On.LevelPlayerMotor.ChaliceDoubleJump += ChaliceDoubleJump;
//            On.LevelPlayerMotor.HandleParry += HandleParry;
//            On.LevelPlayerMotor.HandleDash += HandleDash;
//            On.LevelPlayerMotor.ResetChaliceDoubleJump += ResetChaliceDoubleJump;
//            On.LevelPlayerMotor.HandleLocked += HandleLocked;
//            On.LevelPlayerMotor.HandleWalking += HandleWalking;
//            On.LevelPlayerMotor.HandleLooking += HandleLooking;
//            On.LevelPlayerMotor.HandleFalling += HandleFalling;
//        }



//        public void BufferInputs(On.LevelPlayerMotor.orig_BufferInputs orig, LevelPlayerMotor self)
//        {
//            if (self.player.input.actions.GetButtonDown(2))
//            {
//                self.BufferInput(LevelPlayerMotor.BufferedInput.Jump);
//            }
//            else if (self.player.input.actions.GetButtonDown(7) && !self.dashManager.IsDashing)
//            {
//                self.BufferInput(LevelPlayerMotor.BufferedInput.Dash);
//            }
//            else if (self.player.input.actions.GetButtonDown(4))
//            {
//                self.BufferInput(LevelPlayerMotor.BufferedInput.Super);
//            }
//        }



//        private void HandleJumping(On.LevelPlayerMotor.orig_HandleJumping orig, LevelPlayerMotor self)
//        {
//            if (self.allowJumping)
//            {
//                if (self.jumpManager.state == LevelPlayerMotor.JumpManager.State.Ready && (self.player.input.actions.GetButtonDown(2) || self.HasBufferedInput(LevelPlayerMotor.BufferedInput.Jump)))
//                {
//                    self.hardExitParry = false;
//                    self.ClearBufferedInput();
//                    if (((self.player.stats.ReverseTime > 0f) ? (self.LookDirection.y > 0) : (self.LookDirection.y < 0)) && self.Grounded && self.transform.parent != null)
//                    {
//                        LevelPlatform component = self.transform.parent.GetComponent<LevelPlatform>();
//                        if (component.canFallThrough)
//                        {
//                            self.platformManager.Ignore(self.transform.parent);
//                            self.jumpManager.state = LevelPlayerMotor.JumpManager.State.Used;
//                            self.LeaveGround(false);
//                            self.jumpManager.timeSinceDownJump = 0f;
//                            return;
//                        }
//                    }
//                    AudioManager.Play("player_jump");
//                    self.jumpManager.state = LevelPlayerMotor.JumpManager.State.Hold;
//                    self.LeaveGround(false);
//                    self.velocityManager.y = self.jumpPower;
//                    self.jumpManager.timer = CupheadTime.FixedDelta;
//                    if (self.OnJumpEvent != null)
//                    {
//                        self.OnJumpEvent();
//                    }
//                }
//                if (self.jumpManager.state == LevelPlayerMotor.JumpManager.State.Hold)
//                {
//                    if (!self.directionManager.up.able || (self.jumpManager.timer >= self.properties.jumpHoldMin && (self.player.input.actions.GetButtonUp(2) || !self.player.input.actions.GetButton(2))) || self.jumpManager.timer >= self.properties.jumpHoldMax)
//                    {
//                        self.jumpManager.state = LevelPlayerMotor.JumpManager.State.Used;
//                        self.jumpManager.timer = 0f;
//                    }
//                    if (self.player.stats.isChalice)
//                    {
//                        self.velocityManager.y = ((!self.jumpManager.doubleJumped) ? self.properties.chaliceFirstJumpPower : self.properties.chaliceSecondJumpPower);
//                    }
//                    else
//                    {
//                        self.velocityManager.y = self.jumpPower;
//                    }
//                    self.jumpManager.timer += CupheadTime.FixedDelta;
//                }
//                self.jumpManager.timeSinceDownJump += CupheadTime.FixedDelta;
//                if (self.player.stats.isChalice && !self.jumpManager.doubleJumped)
//                {
//                    self.ChaliceDoubleJump();
//                }
//            }
//        }


//        private void ChaliceDoubleJump(On.LevelPlayerMotor.orig_ChaliceDoubleJump orig, LevelPlayerMotor self)
//        {
//            if ((self.player.input.actions.GetButtonDown(2) || self.HasBufferedInput(LevelPlayerMotor.BufferedInput.Jump)) && self.jumpManager.state == LevelPlayerMotor.JumpManager.State.Used && !self.IsHit)
//            {
//                self.hardExitParry = false;
//                self.ClearBufferedInput();
//                if (self.dashManager.state == LevelPlayerMotor.DashManager.State.End && self.parryManager.state == LevelPlayerMotor.ParryManager.State.Ready)
//                {
//                    self.dashManager.state = LevelPlayerMotor.DashManager.State.Ready;
//                }
//                AudioManager.Play("chalice_doublejump");
//                self.jumpManager.state = LevelPlayerMotor.JumpManager.State.Hold;
//                self.LeaveGround(false);
//                self.jumpManager.doubleJumped = true;
//                self.velocityManager.y = self.properties.chaliceSecondJumpPower;
//                self.jumpManager.timer = CupheadTime.FixedDelta;
//                self.ChaliceDoubleJumped = true;
//                self.platformManager.ResetAll();
//                if (self.OnJumpEvent != null)
//                {
//                    self.OnJumpEvent();
//                }
//                if (self.OnDoubleJumpEvent != null)
//                {
//                    self.OnDoubleJumpEvent();
//                }
//            }
//        }

//        private void HandleParry(On.LevelPlayerMotor.orig_HandleParry orig, LevelPlayerMotor self)
//        {
//            if (self.player.stats.isChalice)
//            {
//                return;
//            }
//            if (self.IsHit)
//            {
//                return;
//            }
//            if (self.parryManager.state == LevelPlayerMotor.ParryManager.State.Ready && (self.player.input.actions.GetButtonDown(2) || self.HasBufferedInput(LevelPlayerMotor.BufferedInput.Jump)) && self.jumpManager.state != LevelPlayerMotor.JumpManager.State.Ready && !self.IsHit)
//            {
//                self.ClearBufferedInput();
//                self.hitManager.state = LevelPlayerMotor.HitManager.State.Inactive;
//                self.parryManager.state = LevelPlayerMotor.ParryManager.State.NotReady;
//                if (self.dashManager.IsDashing)
//                {
//                    self.dashManager.state = LevelPlayerMotor.DashManager.State.End;
//                }
//                self.Parrying = true;
//                if (self.OnParryEvent != null)
//                {
//                    self.OnParryEvent();
//                }
//            }
//        }


//        private bool HandleDash(On.LevelPlayerMotor.orig_HandleDash orig, LevelPlayerMotor self)
//        {
//            if (self.dashManager.state == LevelPlayerMotor.DashManager.State.Ready && (!self.Grounded || self.dashManager.timeSinceGroundDash > 0.1f) && (self.player.input.actions.GetButtonDown(7) || self.HasBufferedInput(LevelPlayerMotor.BufferedInput.Dash)))
//            {
//                self.ClearBufferedInput();
//                AudioManager.Play("player_dash");
//                self.dashManager.state = LevelPlayerMotor.DashManager.State.Start;
//                self.dashManager.direction = self.TrueLookDirection.x;
//                self.dashManager.groundDash = self.Grounded;
//                self.ChaliceDuckDashed = (self.player.stats.isChalice && self.Ducking);
//                if (self.jumpManager.state == LevelPlayerMotor.JumpManager.State.Hold)
//                {
//                    self.jumpManager.state = LevelPlayerMotor.JumpManager.State.Used;
//                }
//                if (self.OnDashStartEvent != null)
//                {
//                    self.OnDashStartEvent();
//                }
//                self.velocityManager.move = 0f;
//                return true;
//            }
//            if (self.dashManager.state == LevelPlayerMotor.DashManager.State.Start)
//            {
//                self.dashManager.state = LevelPlayerMotor.DashManager.State.Dashing;
//            }
//            if (self.player.stats.isChalice && !self.ChaliceDuckDashed)
//            {
//                self.ChaliceDashParry();
//            }
//            if (self.dashManager.state == LevelPlayerMotor.DashManager.State.Dashing)
//            {
//                self.velocityManager.dash = self.properties.dashSpeed * (float)self.dashManager.direction;
//                self.dashManager.timer += CupheadTime.FixedDelta;
//                self.velocityManager.y = 0f;
//                if (self.dashManager.timer >= self.properties.dashTime)
//                {
//                    self.DashComplete();
//                }
//                if (!self.Grounded)
//                {
//                    self.jumpManager.ableToLand = true;
//                }
//                return true;
//            }
//            if (self.dashManager.state == LevelPlayerMotor.DashManager.State.End)
//            {
//                if (self.Grounded)
//                {
//                    self.dashManager.state = LevelPlayerMotor.DashManager.State.Ready;
//                    if (self.dashManager.groundDash)
//                    {
//                        self.dashManager.timeSinceGroundDash = 0f;
//                    }
//                    if (self.player.stats.isChalice)
//                    {
//                        self.dashManager.chaliceParryCoolDown = false;
//                        self.dashManager.chaliceParryCoolDownTimer = 0f;
//                    }
//                }
//                else
//                {
//                    self.dashManager.groundDash = false;
//                }
//                self.ChaliceDuckDashed = false;
//                if (self.player.stats.isChalice && !self.dashManager.chaliceParryCoolDown)
//                {
//                    self.dashManager.state = LevelPlayerMotor.DashManager.State.Ready;
//                }
//                if (self.player.stats.isChalice)
//                {
//                    self.ChaliceDashCooldownCheck();
//                }
//            }
//            return false;
//        }




//        // Token: 0x06007015 RID: 28693 RVA: 0x0003B78A File Offset: 0x0003998A
//        public void ResetChaliceDoubleJump(On.LevelPlayerMotor.orig_ResetChaliceDoubleJump orig, LevelPlayerMotor self)
//        {
//            self.jumpManager.doubleJumped = false;
//            if (self.player.stats.isChalice)
//            {
//                self.dashManager.chaliceParryCoolDown = false;
//                self.dashManager.chaliceParryCoolDownTimer = 0f;
//            }
//        }


//        // Token: 0x06007018 RID: 28696 RVA: 0x0003B7E1 File Offset: 0x000399E1
//        private void HandleLocked(On.LevelPlayerMotor.orig_HandleLocked orig, LevelPlayerMotor self)
//        {
//            if (self.player.input.actions.GetButton(6) && self.Grounded)
//            {
//                self.Locked = true;
//            }
//            else
//            {
//                self.Locked = false;
//            }
//        }

//        // Token: 0x06007019 RID: 28697 RVA: 0x0021DBAC File Offset: 0x0021BDAC
//        private void HandleWalking(On.LevelPlayerMotor.orig_HandleWalking orig, LevelPlayerMotor self)
//        {
//            float move = 0f;
//            if ((self.LookDirection.y >= 0 || !self.Grounded) && !self.Locked)
//            {
//                int num = (self.player.stats.ReverseTime > 0f) ? (-self.player.input.GetAxisInt(PlayerInput.Axis.X, false, false)) : self.player.input.GetAxisInt(PlayerInput.Axis.X, false, false);
//                move = (float)num * self.properties.moveSpeed;
//            }
//            self.velocityManager.move = move;
//        }

//        // Token: 0x0600701A RID: 28698 RVA: 0x0021DC50 File Offset: 0x0021BE50
//        private void HandleLooking(On.LevelPlayerMotor.orig_HandleLooking orig, LevelPlayerMotor self)
//        {
//            if (self.player.levelStarted && self.allowInput)
//            {
//                int num = self.player.input.GetAxisInt(PlayerInput.Axis.X, false, false);
//                num = ((self.player.stats.ReverseTime > 0f) ? (-num) : num);
//                int num2 = self.player.input.GetAxisInt(PlayerInput.Axis.Y, true, self.Grounded && !self.Locked && !self.IsUsingSuperOrEx);
//                num2 = ((self.player.stats.ReverseTime > 0f) ? (-num2) : num2);
//                if (self.GravityReversed)
//                {
//                    num2 *= -1;
//                }
//                self.LookDirection = new Trilean2(num, num2);
//            }
//            int x = self.TrueLookDirection.x;
//            int y = self.TrueLookDirection.y;
//            if (self.LookDirection.x != 0)
//            {
//                x = self.LookDirection.x;
//            }
//            y = self.LookDirection.y;
//            self.TrueLookDirection = new Trilean2(x, y);
//        }



//        // Token: 0x0600701C RID: 28700 RVA: 0x0021DDA0 File Offset: 0x0021BFA0
//        private void HandleFalling(On.LevelPlayerMotor.orig_HandleFalling orig, LevelPlayerMotor self)
//        {
//            if (self.Grounded || self.dashManager.IsDashing)
//            {
//                self.isFloating = false;
//                self.jumpManager.floatTimer = 0f;
//                return;
//            }
//            if (Level.Current.LevelTime < 0.2f)
//            {
//                return;
//            }
//            float num = self.properties.timeToMaxY * 60f;
//            float num2 = self.properties.maxSpeedY / num * CupheadTime.FixedDelta;
//            self.velocityManager.y += num2;
//            self.jumpManager.ableToLand = (self.velocityManager.y > 0f);
//            if (self.player.stats.Loadout.charm == Charm.charm_float && self.jumpManager.ableToLand && self.player.input.actions.GetButton(2) && self.jumpManager.floatTimer < WeaponProperties.CharmFloat.maxTime)
//            {
//                self.isFloating = true;
//                float value = Mathf.Clamp(self.jumpManager.floatTimer - WeaponProperties.CharmFloat.falloffStartTime, 0f, WeaponProperties.CharmFloat.maxTime - WeaponProperties.CharmFloat.falloffStartTime);
//                value = Mathf.InverseLerp(0f, WeaponProperties.CharmFloat.maxTime - WeaponProperties.CharmFloat.falloffStartTime, value);
//                self.velocityManager.y = Mathf.Clamp(self.velocityManager.y, 0f, EaseUtils.EaseInSine(WeaponProperties.CharmFloat.minFallSpeed, WeaponProperties.CharmFloat.maxFallSpeed, value));
//                self.jumpManager.floatTimer += CupheadTime.FixedDelta;
//            }
//            else
//            {
//                self.isFloating = false;
//            }
//        }




//    }

//}