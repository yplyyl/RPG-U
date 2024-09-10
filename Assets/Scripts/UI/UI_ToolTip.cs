using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_ToolTip : MonoBehaviour
{
    [SerializeField] private float xLimit = 960;
    [SerializeField] private float yLimit = 540;

    [SerializeField] private float xOffset = 150;
    [SerializeField] private float yOffset = 150;

    public virtual void AdjustPosition()
    {
        Vector2 mousePosition = Input.mousePosition;

        float _xoffset = 0;
        float _yoffset = 0;

        if (mousePosition.x > xLimit)
            _xoffset = -xOffset;
        else
            _xoffset = xOffset;

        if (mousePosition.y > yLimit)
            _yoffset = -yOffset;
        else
            _yoffset = yOffset;

        transform.position = new Vector2(mousePosition.x + _xoffset, mousePosition.y + _yoffset);
    }
}
