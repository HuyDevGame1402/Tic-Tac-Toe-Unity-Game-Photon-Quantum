namespace Quantum {
  using Photon.Deterministic;
  using UnityEngine;
  using System;
  using System.Collections;


  public class QuantumDebugInput : MonoBehaviour {
    public FP x, y;
    public FP posX, posY;
    public int posArray;
    public bool isClick = false;
    public bool isNewGame = false;
    public bool isNewGameCheckBool = false;
    public bool isGameCompleted = false;
    public bool isStartGame = false;
    private Quantum.Input cachedInput = new Quantum.Input(); // Lưu input
    public Transform gameView;

    private void OnEnable() {
      //Debug.Log("[Quantum] Subscribing to PollInput");
      QuantumCallback.Subscribe(this, (CallbackPollInput callback) => PollInput(callback));
      GridPosition.Click += GetPositionXY;
      ResetGame.OnReset += SignalNewGame;
    }
    public void PollInput(CallbackPollInput callback) {
      if (isClick) {
        Debug.Log("OnClick");
        GetInput(); // Cập nhật cachedInput
        //Debug.Log($"[Quantum] PollInput FINAL - Sending: x = {cachedInput.x}, y = {cachedInput.y}");
        callback.SetInput(cachedInput, DeterministicInputFlags.Repeatable);
        isClick = false;
      }
      if (isNewGameCheckBool) {
        GetInputNewGame();
        isNewGameCheckBool = false;
        isNewGame = false;
        callback.SetInput(cachedInput, DeterministicInputFlags.Repeatable);
        cachedInput = new Quantum.Input();
        StartCoroutine(WaitNewGame());  
      }
      if (isGameCompleted) {
        isGameCompleted = false;
        cachedInput.NewGame = false;
        callback.SetInput(cachedInput, DeterministicInputFlags.Repeatable);
      }
      if (isStartGame) {
        //cachedInput.NewGame = isStartGame;
        cachedInput.InitGame = true;
        isStartGame = false;
        callback.SetInput(cachedInput, DeterministicInputFlags.Repeatable);
      }
    }
    private IEnumerator WaitNewGame() {
      yield return new WaitForSeconds(1f);
      isGameCompleted = true;
    }
    private void GetInput() {
      cachedInput.x = x;
      cachedInput.y = y;
      cachedInput.PositionCreatePrototype = new FPVector2(posX, posY);
      cachedInput.PosArray = posArray;
      cachedInput.CreateProtoype = true;
      Debug.Log($"Changed input: {cachedInput.x}, {cachedInput.y}, {cachedInput.CreateProtoype}");
    }

    private void GetPositionXY(FP X, FP Y, FP posX, FP posY, int posArray) {
      x = X;
      y = Y;
      this.posX = posX;
      this.posY = posY;
      this.posArray = posArray;
      isClick = true;
      Debug.Log($"[Quantum] GetPositionXY called: x = {x}, y = {y}");
    }
    private void SignalNewGame() {
      isNewGame = true;
      isNewGameCheckBool = true;
    }
    private void GetInputNewGame() {
      cachedInput.NewGame = isNewGame;
      Debug.Log(cachedInput.NewGame);
    }

    public void StartGamePressed() {
      Debug.Log("Chay Game 1");
      isStartGame = true;
      //if(QuantumRunner.Default.Session.PlayerCount == 2) {
      //  isStartGame = true;
      //  Debug.Log("Chay Game");
      //}
    }
  }

}