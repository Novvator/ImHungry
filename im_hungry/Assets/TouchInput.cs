using UnityEngine;

// Wraps Input.touches so the game can also be played with a mouse
// (WebGL in a desktop browser, or the Editor without a device).
// Real touches always win; with no touches, the left mouse button acts as finger 0.
public static class TouchInput
{
    static readonly Touch[] none = new Touch[0];
    static readonly Touch[] mouseTouch = new Touch[1];
    static Vector2 lastMousePosition;
    static int cachedFrame = -1;
    static Touch[] cached = none;

    public static Touch[] touches
    {
        get
        {
            if (cachedFrame == Time.frameCount) return cached;
            cachedFrame = Time.frameCount;
            cached = Read();
            return cached;
        }
    }

    static Touch[] Read()
    {
        if (Input.touchCount > 0) return Input.touches;

        Vector2 position = Input.mousePosition;
        TouchPhase phase;
        if (Input.GetMouseButtonDown(0)) phase = TouchPhase.Began;
        else if (Input.GetMouseButtonUp(0)) phase = TouchPhase.Ended;
        else if (Input.GetMouseButton(0)) phase = position == lastMousePosition ? TouchPhase.Stationary : TouchPhase.Moved;
        else
        {
            lastMousePosition = position;
            return none;
        }

        Touch touch = new Touch();
        touch.fingerId = 0;
        touch.position = position;
        touch.deltaPosition = phase == TouchPhase.Began ? Vector2.zero : position - lastMousePosition;
        touch.phase = phase;
        touch.tapCount = 1;
        lastMousePosition = position;

        mouseTouch[0] = touch;
        return mouseTouch;
    }
}
