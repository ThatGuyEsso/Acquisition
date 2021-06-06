﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public interface IManager
{
    void BindToGameStateManager();
    void EvaluateGameState(GameState newState);
}
