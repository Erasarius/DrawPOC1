using UnityEngine;
using System.Collections;
using KanjiDraw;

public class UIKanjiBuilder : MonoBehaviour {
    private bool viewCorners = false;

    public void onNextClick() {
        Messenger.Broadcast(GameEvent.NEXT_STROKE);
    }

    public void onAddCorners() {
        Messenger.Broadcast(GameEvent.ADD_CORNERS);
    }

    public void onResetCharacter() {
        Messenger.Broadcast(GameEvent.RESET_CHARACTER);
    }

    public void onSaveCharacter() {
        Messenger.Broadcast(GameEvent.SAVE_CHARACTER);
    }

    public void onViewCorners() {
        viewCorners = viewCorners ? false : true;

        Messenger<bool>.Broadcast(GameEvent.VIEW_CORNERS, viewCorners);
    }
}
