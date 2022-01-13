﻿using System;
using System.Collections.Generic;

class Game
{
    public static readonly string Title = "IceClimber";
    public static readonly Vector2 Resolution = new Vector2(640, 480);

    readonly Texture _background = Engine.LoadTexture("Background.jpg");

    readonly Texture _sprite = Engine.LoadTexture("player.png");

    readonly Font font = Engine.LoadFont("OpenSans-Regular.ttf", 10);

    readonly Texture prizeSkin = Engine.LoadTexture("eastern_orthodox_cross.png");

    int spriteX = 320;
    int spriteY = 280;


    Enemy enemy1 = new Enemy();
    Boolean enemy1OnScreen = false;
    Boolean enemy1MoveLeft = true;

    int spriteSizeX = 20;
    int spriteSizeY = 25;


    readonly Texture _block = Engine.LoadTexture("square.png");

    List<int> blocksX = new List<int>();
    List<int> blocksY = new List<int>();
    //amount of time block needs to be hit before breaking
    List<int> blockHitCount = new List<int>();


    //bonus trinkets
    readonly Texture trinketSkin = Engine.LoadTexture("eastern_orthodox_cross.png");
    List<int> trinketX = new List<int>();
    List<int> trinketY = new List<int>();
    int trinketSizeX = 20;
    int trinketSizeY = 30;

    int totalPoints = 0;

    Boolean gameOver = false;
    public Game()
    {
        addInitialLayers();
    }

    public void Update()
    {
        Engine.DrawTexture(_background, Vector2.Zero);

        if (playerHitsBorders()) 
        {
            Engine.DrawString("HITTING BORDERS", new Vector2(10, 440), Color.Red, font);
        }



        //draw levels
        for (int i = 0; i < blocksX.Count; i++)
        {
            
             Vector2 vec = new Vector2(blocksX[i], blocksY[i]);
             Engine.DrawTexture(_block, vec, null, new Vector2(spriteSizeX, spriteSizeY));
            
        }

        //if statement inside addLayer to see if a layer sould be added
        addLayer();

        Engine.DrawTexture(_sprite, new Vector2(spriteX, spriteY), null, new Vector2(20, 25));

        if (Engine.GetKeyDown(Key.Left, true))
        {
            spriteX -= 10;
        }
        else if (Engine.GetKeyDown(Key.Right, true))
        {
            spriteX += 10;
        }
        else if (Engine.GetKeyDown(Key.Up, true))
        {
            spriteY -= 10;
        }
        else if (Engine.GetKeyDown(Key.Down, true)) 
        {
            spriteY += 10;
        }
        // shifts everything down
        if (spriteY == Resolution.Y / 2) 
        {
            for (int i = 0; i < blocksY.Count; i++) 
            {
                blocksY[i] = blocksY[i] + 10;
            }

            //fixing the trinkets
            for (int i = 0; i < trinketY.Count; i++)
            {
                trinketY[i] = trinketY[i] + 10;
            }

            spriteY += 20;
            if (enemy1OnScreen)
            {
                enemy1.setEnemyY(enemy1.getEnemyY() + 10);
               
            }
        }

        if (enemy1OnScreen)
        {
            
            enemy1.drawEnemy();
            if(!enemy1MoveLeft)
            {
                enemy1.setEnemyX(enemy1.getEnemyX() + 1);
                if (enemy1.getEnemyX() == enemy1.getInitialX())
                {
                    enemy1MoveLeft = true;
                }
            } 
            else if(enemy1MoveLeft)
            {
                enemy1.setEnemyX(enemy1.getEnemyX() - 1);
                if(enemy1.getEnemyX() < enemy1.getInitialX() - 50)
                {
                    enemy1MoveLeft = false;
                }
            }
            
        }
        if (playerIsOverlapping()) 
        {
            Engine.DrawString("OVERLAPPING", new Vector2(10, 440), Color.Red, font);
        }

        //displaying the number of points
        Engine.DrawString(totalPoints.ToString(), new Vector2(440, 440), Color.Red, font);


        //trinket code

        //draw all the trinkets
        for (int i = 0; i < trinketX.Count; i++)
        {
            Vector2 vec = new Vector2(trinketX[i], trinketY[i]);
            Engine.DrawTexture(trinketSkin, vec, null, new Vector2(trinketSizeX, trinketSizeY));

        }

        //collect trinkets
        for(int i = 0; i < trinketX.Count; i++)
        {
            Bounds2 trinketBounds = new Bounds2(trinketX[i], trinketY[i], trinketSizeX, trinketSizeY);

            Bounds2 playerBounds = new Bounds2(spriteX, spriteY, spriteSizeX, spriteSizeY);

            if (playerBounds.Overlaps(trinketBounds)) 
            {
                trinketX.RemoveAt(i);
                trinketY.RemoveAt(i);
                totalPoints += 50;
            }            
        }

    }

    public void addInitialLayers() 
    {
        for(int y = 0; y < (int)Resolution.Y; y += 100) 
        {
            for (int i = 0; i <= Resolution.X; i += 20)
            {
                blocksX.Add(i);
                blocksY.Add(y);
                blockHitCount.Add(4);
            }
        }
    }
    public void addLayer() 
    {
        // create initial layer
        if (blocksY.Count == 0)
        {
            blocksX.Add(0);
            blocksY.Add(0);
            blockHitCount.Add(4);

            blocksX.Add(620);
            blocksY.Add(0);
            blockHitCount.Add(4);

            totalPoints++;
        }
        // create new layer
        else if (blocksY[blocksY.Count - 2] > 100 && blocksY[blocksY.Count - 1] > 100) 
        {
            Random rand = new Random();            
            int bound = rand.Next(0, (int)Resolution.X);

            while (bound % 20 != 0) 
            {
                bound = rand.Next(0, (int)Resolution.X - 60); 
            }

            for (int i = 0; i < bound; i += 20)
            {
                blocksX.Add(i);
                blocksY.Add(0);
                blockHitCount.Add(4);
            }

            for (int i = bound + 60; i <= Resolution.X; i += 20) 
            {
                blocksX.Add(i);
                blocksY.Add(0);
                blockHitCount.Add(4);
            }

            totalPoints++;

            if(!enemy1OnScreen && rand.Next(1, 5) == 3)
            {
                enemy1 = new Enemy();
                enemy1MoveLeft = true;
                enemy1OnScreen = true;
            }
            if(enemy1.getEnemyY() > 640)
            {
                enemy1OnScreen = false;
            }
              
                        


            createTrinket();

        }
        
    }
    
    //check if the player overlaps with any blocks
    public bool playerIsOverlapping() 
    {

        Bounds2 spritePosition = new Bounds2(new Vector2(spriteX, spriteY), new Vector2(20, 25));

        for (int i = 0; i < blocksX.Count; i++)
        {
            Bounds2 floorBounds = new Bounds2(new Vector2(blocksX[i], blocksY[i]), new Vector2(20, 20));
            if (spritePosition.Overlaps(floorBounds))
            {
                //correct the error
                if (spriteY < blocksY[i])
                {
                    spriteY -= 10;
                    blockHitCount[i] = blockHitCount[i] - 1;
                }
                else if (spriteY > blocksY[i]) 
                {
                    spriteY += 10;
                    blockHitCount[i] = blockHitCount[i] - 1;
                }

                if (blockHitCount[i] == 0) 
                {
                    blocksX.RemoveAt(i);
                    blocksY.RemoveAt(i);
                    blockHitCount.RemoveAt(i);

                    //lose points for breaking a block rather than going through the hole.
                    totalPoints -= 4;
                }
                return true;
            }
        }
        Bounds2 enemyBounds = new Bounds2(new Vector2(enemy1.getEnemyX(), enemy1.getEnemyY())
            , new Vector2(29, 29));
        if (spritePosition.Overlaps(enemyBounds))
        {
            gameOver = true;
            totalPoints = -1;
        }


        return false;
    }

    public bool playerHitsBorders() 
    {
        //borders
        if (spriteX < 0)
        {
            spriteX += 10;
            return true;
        }
        if (spriteX + spriteSizeX> (int)Resolution.X)
        {
            spriteX -= 10;
            return true;
        }
        if (spriteY < 0)
        {
            spriteY += 10;
            return true;
        }
        if (spriteY + spriteSizeY > (int)Resolution.Y)
        {
            spriteY -= 10;
            return true;
        }

        return false;       

    }

    //trinket method
    public void createTrinket() 
    {
        Random rand = new Random();
        int bound = rand.Next(0, (int)Resolution.X);

        trinketX.Add(bound);
        trinketY.Add(blocksY[blocksY.Count - 1] - 30);        
    }

    
  


}
