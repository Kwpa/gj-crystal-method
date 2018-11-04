using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settings : MonoBehaviour {

    public GameObject Root;
    public int playerClass = 1;
    public int playerColliderSize = 1;
    public int playerDepth = 0;
    public int playerMovementSpeed = 1;
    public int playerRotationSpeed = 1;
    public int playerHealth = 1;

    public int AIPlayerMovementSpeed = 1;
    public int AIPlayerRotationSpeed = 1;
    public int AIPlayerHealth = 1;

    public float xSpread = 5;
    public float ySpread = 5;
    public int numberOfPlayers = 10;
    public int numberOfAIPlayers = 10;
}
