﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace KismetDataTypes
{
    public static class CollisionManager
    {
        static public Vector2 ResolvePlayerStaticCollisions(Vector2 nextPosition, Vector2 vel, List<MagicItem> magicItemList)
        {
            float xVel = vel.X, yVel = vel.Y;
            
            if (vel.Y >=32)
                yVel = 31;
            if (Math.Abs(vel.X) > 32)
            {
                if (vel.X < 0)
                    vel.X = -31;
                else
                    vel.X = 31;
            }

            // Get the player's bounding rectangle and find neighboring tiles.
        
            Rectangle nextBounds = GV.Player.getBounds(nextPosition);
            //Rectangle pBounds = GV.Player.Bounds;
            int leftTile = (int)Math.Floor((float)nextBounds.Left / Layer.TileWidth);
            int rightTile = (int)Math.Ceiling(((float)nextBounds.Right / Layer.TileWidth)) - 1;
            int topTile = (int)Math.Floor((float)nextBounds.Top / Layer.TileHeight);
            int bottomTile = (int)Math.Ceiling(((float)nextBounds.Bottom / Layer.TileHeight)) - 1;

            // Reset flag to search for ground collision.
            GV.Player.IsOnGround = false;

            bool BottomCollision = false;
            bool LeftCollision = false;

            bool XCollision = false;
            Rectangle tileBounds;
            Vector2 depth1 = new Vector2(0.0f,0.0f);
            Vector2 depth2 = new Vector2(0.0f,0.0f);

                        // For each potentially colliding tile,
            for (int y = topTile; y < bottomTile; ++y)
            {
                    
                    int lCollision = GV.Level.GetCollision(leftTile, y);
                    if (lCollision == Layer.Impassable)
                    {
                        LeftCollision = true;
                        XCollision = true;
                    }
                    int rCollision = GV.Level.GetCollision(rightTile, y);

                    if (rCollision == Layer.Impassable)
                    {
                        XCollision = true;
                    }
            }

            if (XCollision)
            {
                if (LeftCollision)
                {
                    tileBounds = GV.Level.GetBounds(leftTile, bottomTile);
                }
                else
                {
                    tileBounds = GV.Level.GetBounds(rightTile, bottomTile);
                }
                depth2 = RectangleExtensions.GetIntersectionDepth(nextBounds, tileBounds);

                xVel = vel.X + depth2.X;

                nextBounds = new Rectangle(nextBounds.X + (int)depth2.X,nextBounds.Y, nextBounds.Width, nextBounds.Height);
                //Rectangle pBounds = GV.Player.Bounds;
                leftTile = (int)Math.Floor((float)nextBounds.Left / Layer.TileWidth);
                rightTile = (int)Math.Ceiling(((float)nextBounds.Right / Layer.TileWidth)) - 1;
                topTile = (int)Math.Floor((float)nextBounds.Top / Layer.TileHeight);
                bottomTile = (int)Math.Ceiling(((float)nextBounds.Bottom / Layer.TileHeight)) - 1;
              
            }  

            if (vel.Y > 0)
            {
                for (int x = leftTile; x <= rightTile; ++x)
                {
                    int collision = GV.Level.GetCollision(x, bottomTile);
                    if (collision != Layer.Passable)
                    {               
                            BottomCollision = true;
                    }

                }
            }
            if (BottomCollision)
            {
                tileBounds = GV.Level.GetBounds(leftTile, bottomTile);
                depth1 = RectangleExtensions.GetIntersectionDepth(nextBounds, tileBounds);
                float absDepthY = Math.Abs(depth1.Y);
                float absDepthX = Math.Abs(depth1.X);
                if (GV.Player.PreviousBottom <= tileBounds.Top)
                    yVel = vel.Y + depth1.Y;
                
                GV.Player.IsOnGround = true;
                
            }




                
            if (magicItemList.Count != 0)
            {
                for (int i = 0; i < magicItemList.Count(); i++)
                {
                    MagicItem magicItem = magicItemList[i];
                    Vector2 ItemCollisionDepth = RectangleExtensions.GetIntersectionDepth(GV.Player.Bounds, magicItem.Bounds);
                    if (ItemCollisionDepth != Vector2.Zero && magicItem.Owner == "enemy")
                    {
                        GV.Player.IsHit = true;
                        magicItem.IsCollision = true;
                    }

                }
            }

            return new Vector2(xVel, yVel);

        }

      
        static public Vector2 ResolveCollisions(Enemy enemy, Vector2 nextPosition, Vector2 vel, List<MagicItem> magicItemList)
        {
            float xVel = vel.X, yVel = vel.Y;

            if (vel.Y >= 32)
                yVel = 31;
            if (Math.Abs(vel.X) > 32)
            {
                if (vel.X < 0)
                    vel.X = -31;
                else
                    vel.X = 31;
            }

            // Get the player's bounding rectangle and find neighboring tiles.

            Rectangle nextBounds = enemy.getBounds(nextPosition);
            //Rectangle pBounds = GV.Player.Bounds;
            int leftTile = (int)Math.Floor((float)nextBounds.Left / Layer.TileWidth);
            int rightTile = (int)Math.Ceiling(((float)nextBounds.Right / Layer.TileWidth)) - 1;
            int topTile = (int)Math.Floor((float)nextBounds.Top / Layer.TileHeight);
            int bottomTile = (int)Math.Ceiling(((float)nextBounds.Bottom / Layer.TileHeight)) - 1;

            // Reset flag to search for ground collision.
            enemy.IsOnGround = false;
            enemy.WallDetected = false;
            bool BottomCollision = false;
            bool LeftCollision = false;
            bool XCollision = false;
            Rectangle tileBounds;
            Vector2 depth;

            if (vel.Y > 0)
            {
                for (int x = leftTile; x <= rightTile; ++x)
                {
                    int collision = GV.Level.GetCollision(x, bottomTile);
                    if (collision != Layer.Passable)
                    {             
                               BottomCollision = true;
                    }

                }
            }
            if (BottomCollision)
            {
                tileBounds = GV.Level.GetBounds(leftTile, bottomTile);
                depth = RectangleExtensions.GetIntersectionDepth(nextBounds, tileBounds);
                float absDepthY = Math.Abs(depth.Y);
                if (enemy.PreviousBottom <= tileBounds.Top)
                    yVel = vel.Y + depth.Y;
                enemy.IsOnGround = true;
            }

            // For each potentially colliding tile,
            for (int y = topTile; y < bottomTile; ++y)
            {

                int lCollision = GV.Level.GetCollision(leftTile, y);
                if (lCollision == Layer.Impassable)
                {
                    LeftCollision = true;
                    XCollision = true;
                }
                int rCollision = GV.Level.GetCollision(rightTile, y);

                if (rCollision == Layer.Impassable)
                {

                    XCollision = true;
                }
            }
            //handles collison in x axis
            if (XCollision)
            {
                if (LeftCollision)
                {
                    tileBounds = GV.Level.GetBounds(leftTile, bottomTile);
                    enemy.Direction = GV.RIGHT;
                }
                else
                {
                    tileBounds = GV.Level.GetBounds(rightTile, bottomTile);
                    enemy.Direction = GV.LEFT;
                }
                depth = RectangleExtensions.GetIntersectionDepth(nextBounds, tileBounds);

                xVel = vel.X + depth.X;
            }

            GV.Player.IsHit = false;
            enemy.IsHit = false;
            enemy.BackStabber = false;
            enemy.FaceOff = false;
            enemy.CollisionDetected = false;
            Rectangle enemySightBounds = enemy.SightRange;
            Rectangle playerBounds = GV.Player.Bounds;
            Rectangle enemyBounds = enemy.Bounds;
            Rectangle playerAttackBounds = GV.Player.AttackBounds;

            //Console.WriteLine("Enemy Health " + enemy.Health);
            Vector2 PlayerAttackDepth = RectangleExtensions.GetIntersectionDepth(enemyBounds, playerAttackBounds);
            if (PlayerAttackDepth != Vector2.Zero)
            {

                if (enemy.State.GetType().ToString() != "KismetDataTypes.KnockedDownState")
                {
                    if (GV.Player.State.GetType().ToString() == "KismetDataTypes.Attack1State" && GV.Player.Sprite.CurrentFrame == 7)
                    {

                        Console.WriteLine("Enemy Health " + enemy.Health);
                        enemy.IsHit = true;
                        enemy.Damage = 10;

                        if (enemy.Direction == GV.Player.Direction)
                        {
                            enemy.BackStabber = true;
                        }
                        else
                        {
                            enemy.FaceOff = true;
                        }
                    }
                }


            }
            // check to see if there is a collision 
            Vector2 sightDepth = RectangleExtensions.GetIntersectionDepth(playerBounds, enemySightBounds);
            if (sightDepth != Vector2.Zero)
            {     
                    enemy.CollisionDetected = true;

            }

          

           

            if (magicItemList.Count != 0)
            {
                for (int i = 0; i < magicItemList.Count(); i++)
                {
                    MagicItem magicItem = magicItemList[i];
                    Vector2 ItemCollisionDepth = RectangleExtensions.GetIntersectionDepth(enemy.Bounds, magicItem.Bounds);
                    if (ItemCollisionDepth != Vector2.Zero && magicItem.Enemy == null && magicItem.State.GetType().ToString() == "KismetDataTypes.InUseState")
                    {
                            Console.WriteLine("Enemy Health " + enemy.Health);
                            enemy.Damage = 50;
                            enemy.IsHit = true;
                    }

                }
            }

            return new Vector2(xVel, yVel);
        }



        static public Vector2 ResolveMagicStaticCollisions(MagicItem item, Vector2 nextPosition, Vector2 vel, List<MagicItem> magicItemList)
        {
            float xVel = vel.X, yVel = vel.Y;

            if (vel.Y >= 32)
                yVel = 31;
            if (Math.Abs(vel.X) > 32)
            {
                if (vel.X < 0)
                    vel.X = -31;
                else
                    vel.X = 31;
            }

            // Get the player's bounding rectangle and find neighboring tiles.

            Rectangle nextBounds = item.getBounds(nextPosition);
            //Rectangle pBounds = GV.Player.Bounds;
            int leftTile = (int)Math.Floor((float)nextBounds.Left / Layer.TileWidth);
            int rightTile = (int)Math.Ceiling(((float)nextBounds.Right / Layer.TileWidth)) - 1;
            int topTile = (int)Math.Floor((float)nextBounds.Top / Layer.TileHeight);
            int bottomTile = (int)Math.Ceiling(((float)nextBounds.Bottom / Layer.TileHeight)) - 1;

            // Reset flag to search for ground collision.
            item.IsOnGround = false;

            bool BottomCollision = false;
            bool LeftCollision = false;

            bool XCollision = false;
            Rectangle tileBounds;
            Vector2 depth1 = new Vector2(0.0f, 0.0f);
            Vector2 depth2 = new Vector2(0.0f, 0.0f);

            // For each potentially colliding tile,
            for (int y = topTile; y < bottomTile; ++y)
            {

                int lCollision = GV.Level.GetCollision(leftTile, y);
                if (lCollision == Layer.Impassable)
                {
                    LeftCollision = true;
                    XCollision = true;
                }
                int rCollision = GV.Level.GetCollision(rightTile, y);

                if (rCollision == Layer.Impassable)
                {
                    XCollision = true;
                }
            }

            if (XCollision && item.State.GetType().ToString() != "KismetDataTypes.InUseState")
            {
                if (LeftCollision)
                {
                    tileBounds = GV.Level.GetBounds(leftTile, bottomTile);
                }
                else
                {
                    tileBounds = GV.Level.GetBounds(rightTile, bottomTile);
                }
                depth2 = RectangleExtensions.GetIntersectionDepth(nextBounds, tileBounds);

                xVel = vel.X + depth2.X;

                nextBounds = new Rectangle(nextBounds.X + (int)depth2.X, nextBounds.Y, nextBounds.Width, nextBounds.Height);
                leftTile = (int)Math.Floor((float)nextBounds.Left / Layer.TileWidth);
                rightTile = (int)Math.Ceiling(((float)nextBounds.Right / Layer.TileWidth)) - 1;
                topTile = (int)Math.Floor((float)nextBounds.Top / Layer.TileHeight);
                bottomTile = (int)Math.Ceiling(((float)nextBounds.Bottom / Layer.TileHeight)) - 1;
                if (item.State.GetType().ToString() == "KismetDataTypes.ArrowState")
                    item.Active = false;
            }

            if (vel.Y > 0)
            {
                for (int x = leftTile; x <= rightTile; ++x)
                {
                    int collision = GV.Level.GetCollision(x, bottomTile);
                    if (collision != Layer.Passable)
                    {
                        BottomCollision = true;
                    }

                }
            }
            if (BottomCollision)
            {
                tileBounds = GV.Level.GetBounds(leftTile, bottomTile);
                depth1 = RectangleExtensions.GetIntersectionDepth(nextBounds, tileBounds);
                float absDepthY = Math.Abs(depth1.Y);
                float absDepthX = Math.Abs(depth1.X);
                if (item.PreviousBottom <= tileBounds.Top)
                    yVel = vel.Y + depth1.Y;

               item.IsOnGround = true;

            }
            return new Vector2(xVel, yVel);
        }

    }
}
