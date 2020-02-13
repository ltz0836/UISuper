using System;
using System.Collections.Generic;
using UnityEngine.Serialization;

namespace UnityEngine.UI
{
	[AddComponentMenu("UI/EvonyImage")]
	public class EvonyImage: Image
	{
		private int preHashCode;
		private Vector4 prePadding;
		private Vector4 overrideSpritePadding
		{
			get
			{
				int id = 0;
				Sprite temp = overrideSprite;
				if(temp != null)
				{
					id = temp.GetHashCode();
				}

				if(id != preHashCode)
				{
					prePadding = temp.GetSpriteInfo().padding;
					preHashCode = id;
				}
				return prePadding;
			}
		}

		protected Vector4 GetDrawingDimensionsNew(bool shouldPreserveAspect)
		{
			var padding = overrideSpritePadding;
			var size = overrideSprite == null ? Vector2.zero : new Vector2(overrideSprite.rect.width, overrideSprite.rect.height);
			size.x += padding.x + padding.z;
			size.y += padding.y + padding.w;

			Rect r = GetPixelAdjustedRect();
			// if(EvonyDebug.enableLog)EvonyDebug.Log(string.Format("r:{2}, size:{0}, padding:{1}", size, padding, r));

            int spriteW = Mathf.RoundToInt(size.x);
            int spriteH = Mathf.RoundToInt(size.y);

            var v = new Vector4(
                    padding.x / spriteW,
                    padding.y / spriteH,
                    (spriteW - padding.z) / spriteW,
                    (spriteH - padding.w) / spriteH);

            if (shouldPreserveAspect && size.sqrMagnitude > 0.0f)
            {
                var spriteRatio = size.x / size.y;
                var rectRatio = r.width / r.height;

                if (spriteRatio > rectRatio)
                {
                    var oldHeight = r.height;
                    r.height = r.width * (1.0f / spriteRatio);
                    r.y += (oldHeight - r.height) * rectTransform.pivot.y;
                }
                else
                {
                    var oldWidth = r.width;
                    r.width = r.height * spriteRatio;
                    r.x += (oldWidth - r.width) * rectTransform.pivot.x;
                }
            }

            v = new Vector4(
                    r.x + r.width * v.x,
                    r.y + r.height * v.y,
                    r.x + r.width * v.z,
                    r.y + r.height * v.w
                    );

            return v;
        }

		private Vector4 GetDrawingDimensionsNewForFill(bool shouldPreserveAspect)
		{
			var padding = overrideSpritePadding;
			var size = overrideSprite == null ? Vector2.zero : new Vector2(overrideSprite.rect.width, overrideSprite.rect.height);
			size.x += padding.x + padding.z;
			size.y += padding.y + padding.w;
			
			Rect r = GetPixelAdjustedRect();
			// if(EvonyDebug.enableLog)EvonyDebug.Log(string.Format("r:{2}, size:{0}, padding:{1}", size, padding, r));
			
			int spriteW = Mathf.RoundToInt(size.x);
			int spriteH = Mathf.RoundToInt(size.y);
			
			var v = new Vector4(
				0 / spriteW,
				0 / spriteH,
				(spriteW) / spriteW,
				(spriteH) / spriteH);
			
			if (shouldPreserveAspect && size.sqrMagnitude > 0.0f)
			{
				var spriteRatio = size.x / size.y;
				var rectRatio = r.width / r.height;
				
				if (spriteRatio > rectRatio)
				{
					var oldHeight = r.height;
					r.height = r.width * (1.0f / spriteRatio);
					r.y += (oldHeight - r.height) * rectTransform.pivot.y;
				}
				else
				{
					var oldWidth = r.width;
					r.width = r.height * spriteRatio;
					r.x += (oldWidth - r.width) * rectTransform.pivot.x;
				}
			}
			
			v = new Vector4(
				r.x + r.width * v.x,
				r.y + r.height * v.y,
				r.x + r.width * v.z,
				r.y + r.height * v.w
				);
			
			return v;
		}
			
		protected override void OnPopulateMesh(VertexHelper vh)
		{
			if (overrideSprite == null)
			{
				base.OnPopulateMesh(vh);
				return;
			}

			List<UIVertex> vbo = null;
			List<int> ibo = null;
			if(type != Type.Filled)
			{
				vbo = new List<UIVertex>();
				ibo = new List<int>();
				vh.Clear();
				vh.GetUIVertexStream(vbo);
			}

			switch (type)
			{
			case Type.Simple:
				GenerateSimpleSpriteNew(vbo, ibo, this.preserveAspect);
				break;
			case Type.Sliced:
				GenerateSlicedSpriteNew(vbo, ibo);
				break;
			case Type.Tiled:
				GenerateTiledSpriteNew(vbo, ibo);
				break;
			case Type.Filled:
				//				GenerateFilledSprite(vbo, this.preserveAspect);
				base.OnPopulateMesh(vh);
				return;
			}

			if(type != Type.Filled)
			{
				vh.Clear();
				vh.AddUIVertexStream(vbo, ibo);
			}
		}

		void GenerateSimpleSpriteNew(List<UIVertex> vbo, List<int> ibo, bool preserveAspect)
        {
            var vert = UIVertex.simpleVert;
            vert.color = color;

            Vector4 v = GetDrawingDimensionsNew(preserveAspect);
            var uv = (overrideSprite != null) ? Sprites.DataUtility.GetOuterUV(overrideSprite) : Vector4.zero;

            vert.position = new Vector3(v.x, v.y);
            vert.uv0 = new Vector2(uv.x, uv.y);
            vbo.Add(vert);

            vert.position = new Vector3(v.x, v.w);
            vert.uv0 = new Vector2(uv.x, uv.w);
            vbo.Add(vert);

            vert.position = new Vector3(v.z, v.w);
            vert.uv0 = new Vector2(uv.z, uv.w);
            vbo.Add(vert);

            vert.position = new Vector3(v.z, v.y);
            vert.uv0 = new Vector2(uv.z, uv.y);
            vbo.Add(vert);

			if(ibo != null)
			{
				ibo.Add(0);
				ibo.Add(1);
				ibo.Add(2);
				ibo.Add(0);
				ibo.Add(2);
				ibo.Add(3);
			}
        }
//
//        /// <summary>
//        /// Generate vertices for a 9-sliced Image.
//        /// </summary>
//
        static readonly Vector2[] s_VertScratch = new Vector2[4];
        static readonly Vector2[] s_UVScratch = new Vector2[4];
		void GenerateSlicedSpriteNew(List<UIVertex> vbo, List<int> ibo)
        {
            if (!hasBorder)
            {
                GenerateSimpleSpriteNew(vbo, ibo, false);
                return;
            }

            Vector4 outer, inner, padding, border;

            if (overrideSprite != null)
            {
                outer = Sprites.DataUtility.GetOuterUV(overrideSprite);
                inner = Sprites.DataUtility.GetInnerUV(overrideSprite);
				padding = this.overrideSpritePadding;
                border = overrideSprite.border;
				for(int i = 0; i < 4; i++)
				{
					border[i] += padding[i];
				}
            }
            else
            {
                outer = Vector4.zero;
                inner = Vector4.zero;
                padding = Vector4.zero;
                border = Vector4.zero;
            }

            Rect rect = GetPixelAdjustedRect();
            border = GetAdjustedBorders(border / pixelsPerUnit, rect);
            padding = padding / pixelsPerUnit;

            s_VertScratch[0] = new Vector2(padding.x, padding.y);
            s_VertScratch[3] = new Vector2(rect.width - padding.z, rect.height - padding.w);

            s_VertScratch[1].x = border.x;
            s_VertScratch[1].y = border.y;
            s_VertScratch[2].x = rect.width - border.z;
            s_VertScratch[2].y = rect.height - border.w;

            for (int i = 0; i < 4; ++i)
            {
                s_VertScratch[i].x += rect.x;
                s_VertScratch[i].y += rect.y;
            }

            s_UVScratch[0] = new Vector2(outer.x, outer.y);
            s_UVScratch[1] = new Vector2(inner.x, inner.y);
            s_UVScratch[2] = new Vector2(inner.z, inner.w);
            s_UVScratch[3] = new Vector2(outer.z, outer.w);

            var uiv = UIVertex.simpleVert;
            uiv.color = color;
            for (int x = 0; x < 3; ++x)
            {
                int x2 = x + 1;

                for (int y = 0; y < 3; ++y)
                {
                    if (!this.fillCenter && x == 1 && y == 1)
                    {
                        continue;
                    }

                    int y2 = y + 1;

                    AddQuad(vbo, ibo, uiv,
                        new Vector2(s_VertScratch[x].x, s_VertScratch[y].y),
                        new Vector2(s_VertScratch[x2].x, s_VertScratch[y2].y),
                        new Vector2(s_UVScratch[x].x, s_UVScratch[y].y),
                        new Vector2(s_UVScratch[x2].x, s_UVScratch[y2].y));
                }
            }
        }

//        /// <summary>
//        /// Generate vertices for a tiled Image.
//        /// </summary>
//
		void GenerateTiledSpriteNew(List<UIVertex> vbo, List<int> ibo)
        {
            Vector4 outer, inner, border;
            Vector2 spriteSize;

			Vector4 padding = overrideSpritePadding;
            if (overrideSprite != null)
            {
                outer = Sprites.DataUtility.GetOuterUV(overrideSprite);
                inner = Sprites.DataUtility.GetInnerUV(overrideSprite);
                border = overrideSprite.border;
				if(hasBorder)
				{
					for(int i = 0; i < 4; i++)
					{
						border[i] += padding[i];
					}
				}
                spriteSize = overrideSprite.rect.size;
				spriteSize.x += padding.x + padding.z;
				spriteSize.y += padding.y + padding.w;
            }
            else
            {
                outer = Vector4.zero;
                inner = Vector4.zero;
                border = Vector4.zero;
                spriteSize = Vector2.one * 100;
            }

            Rect rect = GetPixelAdjustedRect();
            float tileWidth = (spriteSize.x - border.x - border.z) / pixelsPerUnit;
            float tileHeight = (spriteSize.y - border.y - border.w) / pixelsPerUnit;
            border = GetAdjustedBorders(border / pixelsPerUnit, rect);

            var uvMin = new Vector2(inner.x, inner.y);
            var uvMax = new Vector2(inner.z, inner.w);

            var v = UIVertex.simpleVert;
            v.color = color;

            // Min to max max range for tiled region in coordinates relative to lower left corner.
            float xMin = border.x;
            float xMax = rect.width - border.z;
            float yMin = border.y;
            float yMax = rect.height - border.w;

            // Safety check. Useful so Unity doesn't run out of memory if the sprites are too small.
            // Max tiles are 100 x 100.
            if ((xMax - xMin) > tileWidth * 100 || (yMax - yMin) > tileHeight * 100)
            {
                tileWidth = (xMax - xMin) / 100;
                tileHeight = (yMax - yMin) / 100;
            }

            var clipped = uvMax;
            if (this.fillCenter)
            {
                for (float y1 = yMin; y1 < yMax; y1 += tileHeight)
                {
                    float y2 = y1 + tileHeight;
                    if (y2 > yMax)
                    {
                        clipped.y = uvMin.y + (uvMax.y - uvMin.y) * (yMax - y1) / (y2 - y1);
                        y2 = yMax;
                    }

                    clipped.x = uvMax.x;
                    for (float x1 = xMin; x1 < xMax; x1 += tileWidth)
                    {
                        float x2 = x1 + tileWidth;
                        if (x2 > xMax)
                        {
                            clipped.x = uvMin.x + (uvMax.x - uvMin.x) * (xMax - x1) / (x2 - x1);
                            x2 = xMax;
                        }

						Vector2 p0 = new Vector2(x1, y1);
						Vector2 p1 = new Vector2(x2, y2);
						if(!hasBorder)
						{
							p0.x += padding[0];
							p0.y += padding[1];
							p1.x -= padding[2];
							p1.y -= padding[3];
						}

						AddQuad(vbo, ibo, v, p0 + rect.position, p1 + rect.position, uvMin, clipped);
                    }
                }
            }

            if (!hasBorder)
                return;

            // Left and right tiled border
            clipped = uvMax;
            for (float y1 = yMin; y1 < yMax; y1 += tileHeight)
            {
                float y2 = y1 + tileHeight;
                if (y2 > yMax)
                {
                    clipped.y = uvMin.y + (uvMax.y - uvMin.y) * (yMax - y1) / (y2 - y1);
                    y2 = yMax;
                }
				AddQuad(vbo, ibo, v,
                    new Vector2(0 + padding[0], y1) + rect.position,
                    new Vector2(xMin, y2) + rect.position,
                    new Vector2(outer.x, uvMin.y),
                    new Vector2(uvMin.x, clipped.y));
				AddQuad(vbo, ibo, v,
                    new Vector2(xMax, y1) + rect.position,
			        new Vector2(rect.width - padding[2], y2) + rect.position,
                    new Vector2(uvMax.x, uvMin.y),
                    new Vector2(outer.z, clipped.y));
            }

            // Bottom and top tiled border
            clipped = uvMax;
            for (float x1 = xMin; x1 < xMax; x1 += tileWidth)
            {
                float x2 = x1 + tileWidth;
                if (x2 > xMax)
                {
                    clipped.x = uvMin.x + (uvMax.x - uvMin.x) * (xMax - x1) / (x2 - x1);
                    x2 = xMax;
                }
				AddQuad(vbo, ibo, v,
			        new Vector2(x1, 0 + padding[1]) + rect.position,
                    new Vector2(x2, yMin) + rect.position,
                    new Vector2(uvMin.x, outer.y),
                    new Vector2(clipped.x, uvMin.y));
				AddQuad(vbo, ibo, v,
                    new Vector2(x1, yMax) + rect.position,
                    new Vector2(x2, rect.height - padding[3]) + rect.position,
                    new Vector2(uvMin.x, uvMax.y),
                    new Vector2(clipped.x, outer.w));
            }

            // Corners
			AddQuad(vbo, ibo, v,
		        new Vector2(0 + padding[0], 0 + padding[1]) + rect.position,
                new Vector2(xMin, yMin) + rect.position,
                new Vector2(outer.x, outer.y),
                new Vector2(uvMin.x, uvMin.y));
			AddQuad(vbo, ibo, v,
		        new Vector2(xMax, 0 + padding[1]) + rect.position,
                new Vector2(rect.width - padding[2], yMin) + rect.position,
                new Vector2(uvMax.x, outer.y),
                new Vector2(outer.z, uvMin.y));
			AddQuad(vbo, ibo, v,
		        new Vector2(0 + padding[0], yMax) + rect.position,
		        new Vector2(xMin, rect.height - padding[3]) + rect.position,
                new Vector2(outer.x, uvMax.y),
                new Vector2(uvMin.x, outer.w));
			AddQuad(vbo, ibo, v,
                new Vector2(xMax, yMax) + rect.position,
		        new Vector2(rect.width - padding[2], rect.height - padding[3]) + rect.position,
                new Vector2(uvMax.x, uvMax.y),
                new Vector2(outer.z, outer.w));
        }

        void AddQuad(List<UIVertex> vbo, List<int> ibo, UIVertex v, Vector2 posMin, Vector2 posMax, Vector2 uvMin, Vector2 uvMax)
        {
			if(ibo != null)
			{
				ibo.Add(vbo.Count + 0);
				ibo.Add(vbo.Count + 1);
				ibo.Add(vbo.Count + 2);
				ibo.Add(vbo.Count + 0);
				ibo.Add(vbo.Count + 2);
				ibo.Add(vbo.Count + 3);
			}

            v.position = new Vector3(posMin.x, posMin.y, 0);
            v.uv0 = new Vector2(uvMin.x, uvMin.y);
            vbo.Add(v);

            v.position = new Vector3(posMin.x, posMax.y, 0);
            v.uv0 = new Vector2(uvMin.x, uvMax.y);
            vbo.Add(v);

            v.position = new Vector3(posMax.x, posMax.y, 0);
            v.uv0 = new Vector2(uvMax.x, uvMax.y);
            vbo.Add(v);

            v.position = new Vector3(posMax.x, posMin.y, 0);
            v.uv0 = new Vector2(uvMax.x, uvMin.y);
            vbo.Add(v);
        }

        Vector4 GetAdjustedBorders(Vector4 border, Rect rect)
        {
            for (int axis = 0; axis <= 1; axis++)
            {
                // If the rect is smaller than the combined borders, then there's not room for the borders at their normal size.
                // In order to avoid artefacts with overlapping borders, we scale the borders down to fit.
                float combinedBorders = border[axis] + border[axis + 2];
                if (rect.size[axis] < combinedBorders && combinedBorders != 0)
                {
                    float borderScaleRatio = rect.size[axis] / combinedBorders;
                    border[axis] *= borderScaleRatio;
                    border[axis + 2] *= borderScaleRatio;
                }
            }
            return border;
        }

        /// <summary>
        /// Generate vertices for a filled Image.
        /// </summary>

        static readonly Vector2[] s_Xy = new Vector2[4];
        static readonly Vector2[] s_Uv = new Vector2[4];
        void GenerateFilledSprite(List<UIVertex> vbo, bool preserveAspect)
        {
            if (this.fillAmount < 0.001f)
                return;

			Vector4 v = GetDrawingDimensionsNewForFill(preserveAspect);
            Vector4 outer = overrideSprite != null ? Sprites.DataUtility.GetOuterUV(overrideSprite) : Vector4.zero;
            UIVertex uiv = UIVertex.simpleVert;
            uiv.color = color;

			var padding = overrideSpritePadding;
			outer.x -= padding.x / overrideSprite.texture.width;
			outer.y -= padding.y / overrideSprite.texture.height;
			outer.z += padding.z / overrideSprite.texture.width;
			outer.w += padding.w / overrideSprite.texture.height;

            float tx0 = outer.x;
            float ty0 = outer.y;
            float tx1 = outer.z;
            float ty1 = outer.w;

			Vector2 size = sprite.rect.size;

            // Horizontal and vertical filled sprites are simple -- just end the Image prematurely
			if (this.fillMethod == FillMethod.Horizontal || this.fillMethod == FillMethod.Vertical)
			{
				//                if (fillMethod == FillMethod.Horizontal)
				//                {
				//                    float fill = (tx1 - tx0) * this.fillAmount;
				//
				//                    if (this.fillOrigin == 1)
				//                    {
				//                        v.x = v.z - (v.z - v.x) * this.fillAmount;
				//                        tx0 = tx1 - fill;
				//                    }
				//                    else
				//                    {
				//                        v.z = v.x + (v.z - v.x) * this.fillAmount;
				//                        tx1 = tx0 + fill;
				//                    }
				//                }
				//                else if (fillMethod == FillMethod.Vertical)
				//                {
				//                    float fill = (ty1 - ty0) * this.fillAmount;
				//
				//                    if (this.fillOrigin == 1)
				//                    {
				//                        v.y = v.w - (v.w - v.y) * this.fillAmount;
				//                        ty0 = ty1 - fill;
				//                    }
				//                    else
				//                    {
				//                        v.w = v.y + (v.w - v.y) * this.fillAmount;
				//                        ty1 = ty0 + fill;
				//                    }
				//                }
				
				if (fillMethod == FillMethod.Horizontal)
				{
					float w0 = size.x;
					float w1 = w0 + padding.z + padding.x;
					float fillPos = this.fillAmount * w1;
					
					float realFillAmount = (fillPos - (this.fillOrigin == 0? padding.x: padding.z)) / w0;
					realFillAmount = Mathf.Clamp01(realFillAmount);
					
					float fill = (tx1 - tx0) * realFillAmount;
					if (this.fillOrigin == 1)
					{
						v.x = v.z - (v.z - v.x) * realFillAmount;
						tx0 = tx1 - fill;
					}
					else
					{
						v.z = v.x + (v.z - v.x) * realFillAmount;
						tx1 = tx0 + fill;
					}
				}
				else if (fillMethod == FillMethod.Vertical)
				{
					float w0 = size.y;
					float w1 = w0 + padding.y + padding.w;
					float fillPos = this.fillAmount * w1;
					
					float realFillAmount = (fillPos - (this.fillOrigin == 0? padding.y: padding.w)) / w0;
					realFillAmount = Mathf.Clamp01(realFillAmount);
					
					float fill = (ty1 - ty0) * realFillAmount;
					if (this.fillOrigin == 1)
					{
						v.y = v.w - (v.w - v.y) * realFillAmount;
						ty0 = ty1 - fill;
					}
					else
					{
						v.w = v.y + (v.w - v.y) * realFillAmount;
						ty1 = ty0 + fill;
					}
				}
				
				v.x += padding.x;
				v.y += padding.y;
				v.z -= padding.z;
				v.w -= padding.w;
				
				
				tx0 += padding.x / overrideSprite.texture.width;
				ty0 += padding.y / overrideSprite.texture.height;
				tx1 -= padding.z / overrideSprite.texture.width;
				ty1 -= padding.w / overrideSprite.texture.height;
			}

            s_Xy[0] = new Vector2(v.x, v.y);
            s_Xy[1] = new Vector2(v.x, v.w);
            s_Xy[2] = new Vector2(v.z, v.w);
            s_Xy[3] = new Vector2(v.z, v.y);

            s_Uv[0] = new Vector2(tx0, ty0);
            s_Uv[1] = new Vector2(tx0, ty1);
            s_Uv[2] = new Vector2(tx1, ty1);
            s_Uv[3] = new Vector2(tx1, ty0);

            if (this.fillAmount < 1f)
            {
                if (fillMethod == FillMethod.Radial90)
                {
                    if (RadialCut(s_Xy, s_Uv, this.fillAmount, this.fillClockwise, this.fillOrigin))
                    {
                        for (int i = 0; i < 4; ++i)
                        {
                            uiv.position = s_Xy[i];
                            uiv.uv0 = s_Uv[i];
                            vbo.Add(uiv);
                        }
                    }
                    return;
                }

                if (fillMethod == FillMethod.Radial180)
                {
                    for (int side = 0; side < 2; ++side)
                    {
                        float fx0, fx1, fy0, fy1;
                        int even = this.fillOrigin > 1 ? 1 : 0;

                        if (this.fillOrigin == 0 || this.fillOrigin == 2)
                        {
                            fy0 = 0f;
                            fy1 = 1f;
                            if (side == even) { fx0 = 0f; fx1 = 0.5f; }
                            else { fx0 = 0.5f; fx1 = 1f; }
                        }
                        else
                        {
                            fx0 = 0f;
                            fx1 = 1f;
                            if (side == even) { fy0 = 0.5f; fy1 = 1f; }
                            else { fy0 = 0f; fy1 = 0.5f; }
                        }

                        s_Xy[0].x = Mathf.Lerp(v.x, v.z, fx0);
                        s_Xy[1].x = s_Xy[0].x;
                        s_Xy[2].x = Mathf.Lerp(v.x, v.z, fx1);
                        s_Xy[3].x = s_Xy[2].x;

                        s_Xy[0].y = Mathf.Lerp(v.y, v.w, fy0);
                        s_Xy[1].y = Mathf.Lerp(v.y, v.w, fy1);
                        s_Xy[2].y = s_Xy[1].y;
                        s_Xy[3].y = s_Xy[0].y;

                        s_Uv[0].x = Mathf.Lerp(tx0, tx1, fx0);
                        s_Uv[1].x = s_Uv[0].x;
                        s_Uv[2].x = Mathf.Lerp(tx0, tx1, fx1);
                        s_Uv[3].x = s_Uv[2].x;

                        s_Uv[0].y = Mathf.Lerp(ty0, ty1, fy0);
                        s_Uv[1].y = Mathf.Lerp(ty0, ty1, fy1);
                        s_Uv[2].y = s_Uv[1].y;
                        s_Uv[3].y = s_Uv[0].y;

                        float val = this.fillClockwise ? fillAmount * 2f - side : this.fillAmount * 2f - (1 - side);

                        if (RadialCut(s_Xy, s_Uv, Mathf.Clamp01(val), this.fillClockwise, ((side + this.fillOrigin + 3) % 4)))
                        {
                            for (int i = 0; i < 4; ++i)
                            {
                                uiv.position = s_Xy[i];
                                uiv.uv0 = s_Uv[i];
                                vbo.Add(uiv);
                            }
                        }
                    }
                    return;
                }

                if (fillMethod == FillMethod.Radial360)
                {
                    for (int corner = 0; corner < 4; ++corner)
                    {
                        float fx0, fx1, fy0, fy1;

                        if (corner < 2) { fx0 = 0f; fx1 = 0.5f; }
                        else { fx0 = 0.5f; fx1 = 1f; }

                        if (corner == 0 || corner == 3) { fy0 = 0f; fy1 = 0.5f; }
                        else { fy0 = 0.5f; fy1 = 1f; }

                        s_Xy[0].x = Mathf.Lerp(v.x, v.z, fx0);
                        s_Xy[1].x = s_Xy[0].x;
                        s_Xy[2].x = Mathf.Lerp(v.x, v.z, fx1);
                        s_Xy[3].x = s_Xy[2].x;

                        s_Xy[0].y = Mathf.Lerp(v.y, v.w, fy0);
                        s_Xy[1].y = Mathf.Lerp(v.y, v.w, fy1);
                        s_Xy[2].y = s_Xy[1].y;
                        s_Xy[3].y = s_Xy[0].y;

                        s_Uv[0].x = Mathf.Lerp(tx0, tx1, fx0);
                        s_Uv[1].x = s_Uv[0].x;
                        s_Uv[2].x = Mathf.Lerp(tx0, tx1, fx1);
                        s_Uv[3].x = s_Uv[2].x;

                        s_Uv[0].y = Mathf.Lerp(ty0, ty1, fy0);
                        s_Uv[1].y = Mathf.Lerp(ty0, ty1, fy1);
                        s_Uv[2].y = s_Uv[1].y;
                        s_Uv[3].y = s_Uv[0].y;

                        float val = this.fillClockwise ?
                            this.fillAmount * 4f - ((corner + this.fillOrigin) % 4) :
                            this.fillAmount * 4f - (3 - ((corner + this.fillOrigin) % 4));

                        if (RadialCut(s_Xy, s_Uv, Mathf.Clamp01(val), this.fillClockwise, ((corner + 2) % 4)))
                        {
                            for (int i = 0; i < 4; ++i)
                            {
                                uiv.position = s_Xy[i];
                                uiv.uv0 = s_Uv[i];
                                vbo.Add(uiv);
                            }
                        }
                    }
                    return;
                }
            }

            // Fill the buffer with the quad for the Image
            for (int i = 0; i < 4; ++i)
            {
                uiv.position = s_Xy[i];
                uiv.uv0 = s_Uv[i];
                vbo.Add(uiv);
            }
        }

        /// <summary>
        /// Adjust the specified quad, making it be radially filled instead.
        /// </summary>

        static bool RadialCut(Vector2[] xy, Vector2[] uv, float fill, bool invert, int corner)
        {
            // Nothing to fill
            if (fill < 0.001f) return false;

            // Even corners invert the fill direction
            if ((corner & 1) == 1) invert = !invert;

            // Nothing to adjust
            if (!invert && fill > 0.999f) return true;

            // Convert 0-1 value into 0 to 90 degrees angle in radians
            float angle = Mathf.Clamp01(fill);
            if (invert) angle = 1f - angle;
            angle *= 90f * Mathf.Deg2Rad;

            // Calculate the effective X and Y factors
            float cos = Mathf.Cos(angle);
            float sin = Mathf.Sin(angle);

            RadialCut(xy, cos, sin, invert, corner);
            RadialCut(uv, cos, sin, invert, corner);
            return true;
        }

        /// <summary>
        /// Adjust the specified quad, making it be radially filled instead.
        /// </summary>

        static void RadialCut(Vector2[] xy, float cos, float sin, bool invert, int corner)
        {
            int i0 = corner;
            int i1 = ((corner + 1) % 4);
            int i2 = ((corner + 2) % 4);
            int i3 = ((corner + 3) % 4);

            if ((corner & 1) == 1)
            {
                if (sin > cos)
                {
                    cos /= sin;
                    sin = 1f;

                    if (invert)
                    {
                        xy[i1].x = Mathf.Lerp(xy[i0].x, xy[i2].x, cos);
                        xy[i2].x = xy[i1].x;
                    }
                }
                else if (cos > sin)
                {
                    sin /= cos;
                    cos = 1f;

                    if (!invert)
                    {
                        xy[i2].y = Mathf.Lerp(xy[i0].y, xy[i2].y, sin);
                        xy[i3].y = xy[i2].y;
                    }
                }
                else
                {
                    cos = 1f;
                    sin = 1f;
                }

                if (!invert) xy[i3].x = Mathf.Lerp(xy[i0].x, xy[i2].x, cos);
                else xy[i1].y = Mathf.Lerp(xy[i0].y, xy[i2].y, sin);
            }
            else
            {
                if (cos > sin)
                {
                    sin /= cos;
                    cos = 1f;

                    if (!invert)
                    {
                        xy[i1].y = Mathf.Lerp(xy[i0].y, xy[i2].y, sin);
                        xy[i2].y = xy[i1].y;
                    }
                }
                else if (sin > cos)
                {
                    cos /= sin;
                    sin = 1f;

                    if (invert)
                    {
                        xy[i2].x = Mathf.Lerp(xy[i0].x, xy[i2].x, cos);
                        xy[i3].x = xy[i2].x;
                    }
                }
                else
                {
                    cos = 1f;
                    sin = 1f;
                }

                if (invert) xy[i3].y = Mathf.Lerp(xy[i0].y, xy[i2].y, sin);
                else xy[i1].x = Mathf.Lerp(xy[i0].x, xy[i2].x, cos);
            }
        }

//        public virtual void CalculateLayoutInputHorizontal() {}
//        public virtual void CalculateLayoutInputVertical() {}

//        public virtual float minWidth { get { return 0; } }

        public override float preferredWidth
        {
            get
            {
                if (overrideSprite == null)
				{
					return 0;
				}
                if (type == Type.Sliced || type == Type.Tiled)
				{
					return Sprites.DataUtility.GetMinSize(overrideSprite).x / pixelsPerUnit;
				}

				var padding = overrideSpritePadding;
				var size = overrideSprite.rect.size;
				size.x += padding.x + padding.z;
				size.y += padding.y + padding.w;
                return size.x / pixelsPerUnit;
            }
        }

//        public virtual float flexibleWidth { get { return -1; } }
//
//        public virtual float minHeight { get { return 0; } }

        public override float preferredHeight
        {
            get
            {
                if (overrideSprite == null)
				{
					return 0;
				}

                if (type == Type.Sliced || type == Type.Tiled)
				{
					return Sprites.DataUtility.GetMinSize(overrideSprite).y / pixelsPerUnit;
				}

				var padding = overrideSpritePadding;
				var size = overrideSprite.rect.size;
				size.x += padding.x + padding.z;
				size.y += padding.y + padding.w;
				return size.y / pixelsPerUnit;
            }
        }

//        public virtual float flexibleHeight { get { return -1; } }
//
//        public virtual int layoutPriority { get { return 0; } }


		public float AlphaThresholdEnovy
		{
			get{
				return alphaHitTestMinimumThreshold;
			}

			set{
				alphaHitTestMinimumThreshold = value;
			}
		}


        public override bool IsRaycastLocationValid(Vector2 screenPoint, Camera eventCamera)
        {
			if (alphaHitTestMinimumThreshold <= 0)
				return true;
			
			if (alphaHitTestMinimumThreshold > 1)
				return false;
			
			if (overrideSprite == null)
				return true;
			
            Sprite sprite = overrideSprite;


            Vector2 local;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, screenPoint, eventCamera, out local);

            Rect rect = GetPixelAdjustedRect();

            // Convert to have lower left corner as reference point.
            local.x += rectTransform.pivot.x * rect.width;
            local.y += rectTransform.pivot.y * rect.height;

            local = MapCoordinate(local, rect);

            // Normalize local coordinates.
            Rect spriteRect = sprite.textureRect;
            Vector2 normalized = new Vector2(local.x / spriteRect.width, local.y / spriteRect.height);

            // Convert to texture space.
            float x = Mathf.Lerp(spriteRect.x, spriteRect.xMax, normalized.x) / sprite.texture.width;
            float y = Mathf.Lerp(spriteRect.y, spriteRect.yMax, normalized.y) / sprite.texture.height;

            try
            {
				return sprite.texture.GetPixelBilinear(x, y).a >= AlphaThresholdEnovy;
				//return activeSprite.texture.GetPixelBilinear(x, y).a >= alphaHitTestMinimumThreshold;
            }
            catch (UnityException e)
            {
                if(EvonyDebug.enableLog)EvonyDebug.LogError("Using clickAlphaThreshold lower than 1 on Image whose sprite texture cannot be read. " + e.Message + " Also make sure to disable sprite packing for this sprite.", this);
                return true;
            }
        }

        private Vector2 MapCoordinate(Vector2 local, Rect rect)
        {
            Rect spriteRect = sprite.rect;
            if (type == Type.Simple || type == Type.Filled)
                return new Vector2(local.x * spriteRect.width / rect.width, local.y * spriteRect.height / rect.height);

            Vector4 border = sprite.border;
            Vector4 adjustedBorder = GetAdjustedBorders(border / pixelsPerUnit, rect);

            for (int i = 0; i < 2; i++)
            {
                if (local[i] <= adjustedBorder[i])
                    continue;

                if (rect.size[i] - local[i] <= adjustedBorder[i + 2])
                {
                    local[i] -= (rect.size[i] - spriteRect.size[i]);
                    continue;
                }

                if (type == Type.Sliced)
                {
                    float lerp = Mathf.InverseLerp(adjustedBorder[i], rect.size[i] - adjustedBorder[i + 2], local[i]);
                    local[i] = Mathf.Lerp(border[i], spriteRect.size[i] - border[i + 2], lerp);
                    continue;
                }
                else
                {
                    local[i] -= adjustedBorder[i];
                    local[i] = Mathf.Repeat(local[i], spriteRect.size[i] - border[i] - border[i + 2]);
                    local[i] += border[i];
                    continue;
                }
            }

            return local;
        }

		public override void SetNativeSize()
		{
			if (overrideSprite != null)
			{
				Vector4 padding = overrideSpritePadding;
				float w = (overrideSprite.rect.width + padding.x + padding.z) / pixelsPerUnit;
				float h = (overrideSprite.rect.height + padding.y + padding.w) / pixelsPerUnit;
				rectTransform.anchorMax = rectTransform.anchorMin;
				rectTransform.sizeDelta = new Vector2(w, h);
				SetAllDirty();
			}
		}
    }
}