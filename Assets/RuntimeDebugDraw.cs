using System;
using System.Collections.Generic;
using UnityEngine;
using RuntimeDebugDraw.Internal;
using Conditional = System.Diagnostics.ConditionalAttribute;

/*
 *	Runtime Debug Draw
 *	Single file debuging DrawLine/DrawText/etc that works in both Scene/Game view, also works in built PC/mobile builds.
 *	
 *	Very Important Notes:
 *	1.	You are expected to make some changes in this file before intergrating this into you projects.
 *			a.	`_DEBUG` symbol, you should this to your project's debugging symbol so these draw calls will be compiled away in final release builds.
 *				If you forget to do this, DrawXXX calls won't be shown.
 *			b.	`RuntimeDebugDraw` namespace and `Draw` class name, you can change this into your project's namespace to make it more accessable.
 *			c.	`Draw.DrawLineLayer` is the layer the lines will be drawn on. If you have camera postprocessing turned on, set this to a layer that is ignored
 *				by the post processor.
 *			d.	`GetDebugDrawCamera()` will be called to get the camera for line drawings and text coordinate calcuation.
 *				It defaults to `Camera.main`, returning null will mute drawings.
 *			e.	`DrawTextDefaultSize`/`DrawDefaultColor` styling variables, defaults as Unity Debug.Draw.
 *	2.	Performance should be relatively ok for debugging,  but it's never intended for release use. You should use conditional to
 *		compile away these calls anyway. Additionally DrawText is implemented with OnGUI, which costs a lot on mobile devices.
 *	3.	Don't rename this file of 'RuntimeDebugDraw' or this won't work. This file contains a MonoBehavior also named 'RuntimeDebugDraw' and Unity needs this file
 *		to have the same name. If you really want to rename this file, remember to rename the 'RuntimeDebugDraw' class below too.
 *	
 *	License: Public Domain
 */

namespace RuntimeDebugDraw
{
	public static class Draw
	{
		#region Main Functions
		/// <summary>
		/// Which layer the lines will be drawn on.
		/// </summary>
		public const int DrawLineLayer = 4;

		/// <summary>
		/// Default font size for DrawText.
		/// </summary>
		public const int DrawTextDefaultSize = 12;

		/// <summary>
		/// Default color for Draws.
		/// </summary>
		public static Color DrawDefaultColor = Color.white;

		/// <summary>
		///	Which camera to use for line drawing and texts coordinate calculation.
		/// </summary>
		/// <returns>Camera to debug draw on, returns null will mute debug drawing.</returns>
		public static Camera GetDebugDrawCamera()
		{
			return Camera.main;
		}

		/// <summary>
		///	Draw a line from <paramref name="start"/> to <paramref name="end"/> with <paramref name="color"/>.
		/// </summary>
		/// <param name="start">Point in world space where the line should start.</param>
		/// <param name="end">Point in world space where the line should end.</param>
		/// <param name="color">Color of the line.</param>
		/// <param name="duration">How long the line should be visible for.</param>
		/// <param name="depthTest">Should the line be obscured by objects closer to the camera?</param>
		[Conditional("_DEBUG")]
		public static void DrawLine(Vector3 start, Vector3 end, Color color, float duration, bool depthTest)
		{
			CheckAndBuildHiddenRTDrawObject();
			_rtDraw.RegisterLine(start, end, color, duration, !depthTest);
			return;
		}

		/// <summary>
		/// Draws a line from start to start + dir in world coordinates.
		/// </summary>
		/// <param name="start">Point in world space where the ray should start.</param>
		/// <param name="dir">Direction and length of the ray.</param>
		/// <param name="color">Color of the drawn line.</param>
		/// <param name="duration">How long the line will be visible for (in seconds).</param>
		/// <param name="depthTest">Should the line be obscured by other objects closer to the camera?</param>
		[Conditional("_DEBUG")]
		public static void DrawRay(Vector3 start, Vector3 dir, Color color, float duration, bool depthTest)
		{
			CheckAndBuildHiddenRTDrawObject();
			_rtDraw.RegisterLine(start, start + dir, color, duration, !depthTest);
			return;
		}

		/// <summary>
		/// Draw a text at given position.
		/// </summary>
		/// <param name="pos">Position</param>
		/// <param name="text">String of the text.</param>
		/// <param name="color">Color for the text.</param>
		/// <param name="size">Font size for the text.</param>
		/// <param name="duration">How long the text should be visible for.</param>
		/// <param name="popUp">Set to true to let the text moving up, so multiple texts at the same position can be visible.</param>
		[Conditional("_DEBUG")]
		public static void DrawText(Vector3 pos, string text, Color color, int size, float duration, bool popUp)
		{
			CheckAndBuildHiddenRTDrawObject();
			_rtDraw.RegisterText(pos, text, color, size, duration, popUp);
			return;
		}
		#endregion

		#region Overloads
		/*
		 *	These are tons of overloads following how 'Debug.DrawXXX' are overloaded.
		 */

		/// <summary>
		///	Draw a line from <paramref name="start"/> to <paramref name="end"/> with <paramref name="color"/>.
		/// </summary>
		/// <param name="start">Point in world space where the line should start.</param>
		/// <param name="end">Point in world space where the line should end.</param>
		[Conditional("_DEBUG")]
		public static void DrawLine(Vector3 start, Vector3 end)
		{
			DrawLine(start, end, DrawDefaultColor, 0f, true);
			return;
		}

		/// <summary>
		///	Draw a line from <paramref name="start"/> to <paramref name="end"/> with <paramref name="color"/>.
		/// </summary>
		/// <param name="start">Point in world space where the line should start.</param>
		/// <param name="end">Point in world space where the line should end.</param>
		/// <param name="color">Color of the line.</param>
		[Conditional("_DEBUG")]
		public static void DrawLine(Vector3 start, Vector3 end, Color color)
		{
			DrawLine(start, end, color, 0f, true);
			return;
		}

		/// <summary>
		///	Draw a line from <paramref name="start"/> to <paramref name="end"/> with <paramref name="color"/>.
		/// </summary>
		/// <param name="start">Point in world space where the line should start.</param>
		/// <param name="end">Point in world space where the line should end.</param>
		/// <param name="color">Color of the line.</param>
		/// <param name="duration">How long the line should be visible for.</param>
		[Conditional("_DEBUG")]
		public static void DrawLine(Vector3 start, Vector3 end, Color color, float duration)
		{
			DrawLine(start, end, color, duration, true);
			return;
		}

		/// <summary>
		/// Draws a line from start to start + dir in world coordinates.
		/// </summary>
		/// <param name="start">Point in world space where the ray should start.</param>
		/// <param name="dir">Direction and length of the ray.</param>
		[Conditional("_DEBUG")]
		public static void DrawRay(Vector3 start, Vector3 dir)
		{
			DrawRay(start, dir, DrawDefaultColor, 0f, true);
			return;
		}

		/// <summary>
		/// Draws a line from start to start + dir in world coordinates.
		/// </summary>
		/// <param name="start">Point in world space where the ray should start.</param>
		/// <param name="dir">Direction and length of the ray.</param>
		/// <param name="color">Color of the drawn line.</param>
		[Conditional("_DEBUG")]
		public static void DrawRay(Vector3 start, Vector3 dir, Color color)
		{
			DrawRay(start, dir, color, 0f, true);
			return;
		}

		/// <summary>
		/// Draws a line from start to start + dir in world coordinates.
		/// </summary>
		/// <param name="start">Point in world space where the ray should start.</param>
		/// <param name="dir">Direction and length of the ray.</param>
		/// <param name="color">Color of the drawn line.</param>
		/// <param name="duration">How long the line will be visible for (in seconds).</param>
		[Conditional("_DEBUG")]
		public static void DrawRay(Vector3 start, Vector3 dir, Color color, float duration)
		{
			DrawRay(start, dir, color, duration, true);
			return;
		}

		/// <summary>
		/// Draw a text at given position.
		/// </summary>
		/// <param name="pos">Position</param>
		/// <param name="text">String of the text.</param>
		[Conditional("_DEBUG")]
		public static void DrawText(Vector3 pos, string text)
		{
			DrawText(pos, text, DrawDefaultColor, DrawTextDefaultSize, 0f, false);
			return;
		}

		/// <summary>
		/// Draw a text at given position.
		/// </summary>
		/// <param name="pos">Position</param>
		/// <param name="text">String of the text.</param>
		/// <param name="color">Color for the text.</param>
		[Conditional("_DEBUG")]
		public static void DrawText(Vector3 pos, string text, Color color)
		{
			DrawText(pos, text, color, DrawTextDefaultSize, 0f, false);
			return;
		}

		/// <summary>
		/// Draw a text at given position.
		/// </summary>
		/// <param name="pos">Position</param>
		/// <param name="text">String of the text.</param>
		/// <param name="color">Color for the text.</param>
		/// <param name="size">Font size for the text.</param>
		[Conditional("_DEBUG")]
		public static void DrawText(Vector3 pos, string text, Color color, int size)
		{
			DrawText(pos, text, color, size, 0f, false);
			return;
		}

		/// <summary>
		/// Draw a text at given position.
		/// </summary>
		/// <param name="pos">Position</param>
		/// <param name="text">String of the text.</param>
		/// <param name="color">Color for the text.</param>
		/// <param name="size">Font size for the text.</param>
		/// <param name="duration">How long the text should be visible for.</param>
		[Conditional("_DEBUG")]
		public static void DrawText(Vector3 pos, string text, Color color, int size, float duration)
		{
			DrawText(pos, text, color, size, duration, false);
			return;
		}
		#endregion

		#region Internal
		/// <summary>
		/// Singleton RuntimeDebugDraw component that is needed to call Unity APIs.
		/// </summary>
		private static Internal.RuntimeDebugDraw _rtDraw;

		/// <summary>
		/// Check and build 
		/// </summary>
		private static void CheckAndBuildHiddenRTDrawObject()
		{
			if (_rtDraw == null)
			{
				//	instantiate an hidden gameobject w/ RuntimeDebugDraw attached.
				//	hardcode an GUID in the name so one won't accidentally get this by name.
				var go = new GameObject("________HIDDEN_C4F6A87F298241078E21C0D7C1D87A76_");
				var childGo = new GameObject("________HIDDEN_9D08E9B9785041CD863FF172480C31B2_");
				childGo.transform.parent = go.transform;
				_rtDraw = childGo.AddComponent<RuntimeDebugDraw.Internal.RuntimeDebugDraw>();
				//	hack to only hide outer go, so that RuntimeDebugDraw's OnGizmos will work properly.
				go.hideFlags = HideFlags.HideInHierarchy;
				GameObject.DontDestroyOnLoad(go);
			}

			return;
		}
		#endregion
	}

	#region Editor
#if UNITY_EDITOR
	[UnityEditor.InitializeOnLoad]
	public static class DrawEditor
	{
		static DrawEditor()
		{
			//	set a low execution order
			var name = typeof(RuntimeDebugDraw.Internal.RuntimeDebugDraw).Name;
			foreach (UnityEditor.MonoScript monoScript in UnityEditor.MonoImporter.GetAllRuntimeMonoScripts())
			{
				if (name != monoScript.name)
					continue;

				if (UnityEditor.MonoImporter.GetExecutionOrder(monoScript) != 9990)
				{
					UnityEditor.MonoImporter.SetExecutionOrder(monoScript, 9990);
					return;
				}
			}
		}
	}
#endif
	#endregion
}

namespace RuntimeDebugDraw.Internal
{
	internal class RuntimeDebugDraw : MonoBehaviour
	{
		#region Basics
		private void Awake()
		{
			_ZTestBatch = new BatchedLineDraw(depthTest: true);
			_AlwaysBatch = new BatchedLineDraw(depthTest: false);
			_lineEntries = new List<DrawLineEntry>(16);

			_textStyle = new GUIStyle();
			_textStyle.alignment = TextAnchor.UpperLeft;
			_textEntries = new List<DrawTextEntry>(16);

			Debug.Log("awka");

			return;
		}

		private void OnGUI()
		{
			DrawTextOnGUI();

			return;
		}

#if UNITY_EDITOR
		private void OnDrawGizmos()
		{
			DrawTextOnDrawGizmos();

			return;
		}
#endif

		//	is this late enough?
		public void LateUpdate()
		{
			TickAndDrawLines();
			TickTexts();

			return;
		}

		private void OnDestroy()
		{
			_AlwaysBatch.Dispose();
			_ZTestBatch.Dispose();

			return;
		}
		#endregion

		#region Draw Lines
		private class DrawLineEntry
		{
			public bool occupied;
			public Vector3 start;
			public Vector3 end;
			public Color color;
			public float timer;
			public bool noZTest;
		}

		private List<DrawLineEntry> _lineEntries;

		//	helper class for batching
		private class BatchedLineDraw : IDisposable
		{
			public Mesh mesh;
			public Material mat;

			private List<Vector3> _vertices;
			private List<Color> _colors;
			private List<int> _indices;

			public BatchedLineDraw(bool depthTest)
			{
				mesh = new Mesh();
				mesh.MarkDynamic();

				//	relying on a builtin shader, but it shouldn't change that much.
				mat = new Material(Shader.Find("Hidden/Internal-Colored"));
				mat.SetInt("_ZTest", depthTest 
					? 4	// LEqual
					: 0	// Always
					);

				_vertices = new List<Vector3>();
				_colors = new List<Color>();
				_indices = new List<int>();

				return;
			}

			public void AddLine(Vector3 from, Vector3 to, Color color)
			{
				_vertices.Add(from);
				_vertices.Add(to);
				_colors.Add(color);
				_colors.Add(color);
				int verticeCount = _vertices.Count;
				_indices.Add(verticeCount - 2);
				_indices.Add(verticeCount - 1);

				return;
			}

			public void Clear()
			{
				mesh.Clear();
				_vertices.Clear();
				_colors.Clear();
				_indices.Clear();

				return;
			}

			public void BuildBatch()
			{
				mesh.SetVertices(_vertices);
				mesh.SetColors(_colors);
				mesh.SetIndices(_indices.ToArray(), MeshTopology.Lines, 0);	// cant get rid of this alloc for now

				return;
			}

			public void Dispose()
			{
				GameObject.Destroy(mesh);
				GameObject.Destroy(mat);
			}
		}

		private BatchedLineDraw _ZTestBatch;
		private BatchedLineDraw _AlwaysBatch;
		private bool _linesNeedRebuild;

		public void RegisterLine(Vector3 start, Vector3 end, Color color, float timer, bool noZTest)
		{
			DrawLineEntry entry = null;
			for (int ix = 0; ix < _lineEntries.Count; ix++)
			{
				if (!_lineEntries[ix].occupied)
				{
					entry = _lineEntries[ix];
					break;
				}
			}
			if (entry == null)
			{
				entry = new DrawLineEntry();
				_lineEntries.Add(entry);
			}

			entry.occupied = true;
			entry.start = start;
			entry.end = end;
			entry.color = color;
			entry.timer = timer;
			entry.noZTest = noZTest;
			_linesNeedRebuild = true;

			return;
		}

		private void RebuildDrawLineBatchMesh()
		{
			_ZTestBatch.Clear();
			_AlwaysBatch.Clear();

			for (int ix = 0; ix < _lineEntries.Count; ix++)
			{
				var entry = _lineEntries[ix];
				if (!entry.occupied)
					continue;

				if (entry.noZTest)
					_AlwaysBatch.AddLine(entry.start, entry.end, entry.color);
				else
					_ZTestBatch.AddLine(entry.start, entry.end, entry.color);
			}

			_ZTestBatch.BuildBatch();
			_AlwaysBatch.BuildBatch();

			return;
		}

		private void TickAndDrawLines()
		{
			if (_linesNeedRebuild)
			{
				RebuildDrawLineBatchMesh();
				_linesNeedRebuild = false;
			}

			//	draw on UI layer which should bypass most postFX setups
			Graphics.DrawMesh(_AlwaysBatch.mesh, Vector3.zero, Quaternion.identity, _AlwaysBatch.mat, layer: Draw.DrawLineLayer ,camera : null, submeshIndex : 0,properties: null, castShadows : false, receiveShadows : false);
			Graphics.DrawMesh(_ZTestBatch.mesh, Vector3.zero, Quaternion.identity, _ZTestBatch.mat, layer: Draw.DrawLineLayer ,camera : null, submeshIndex : 0,properties: null, castShadows : false, receiveShadows : false);

			//	update timer late so every added entry can be drawed for at least one frame
			for (int ix = 0; ix < _lineEntries.Count; ix++)
			{
				var entry = _lineEntries[ix];
				if (!entry.occupied)
					continue;
				entry.timer -= Time.deltaTime;
				if (entry.timer < 0)
				{
					entry.occupied = false;
					_linesNeedRebuild = true;
				}
			}

			return;
		}
		#endregion

		#region Draw Text
		private class DrawTextEntry
		{
			[Flags]
			public enum DrawFlag : byte
			{
				None		= 0,
				DrawnGizmo	= 1 << 0,
				DrawnGUI	= 1 << 1,
				DrawnAll	= DrawnGizmo | DrawnGUI
			}

			public bool occupied;
			public GUIContent content;
			public Vector3 anchor;
			public int size;
			public Color color;
			public float timer;
			public bool popUp;
			public float duration;

			//	Text entries needs to be draw in both OnGUI/OnDrawGizmos, need flags for mark
			//	has been visited by both
			public DrawFlag flag = DrawFlag.None;

			public DrawTextEntry()
			{
				content = new GUIContent();
				return;
			}
		}

		private List<DrawTextEntry> _textEntries;
		private GUIStyle _textStyle;

		public void RegisterText(Vector3 anchor, string text, Color color, int size, float timer, bool popUp)
		{
			DrawTextEntry entry = null;
			for (int ix = 0; ix < _textEntries.Count; ix++)
			{
				if (!_textEntries[ix].occupied)
				{
					entry = _textEntries[ix];
					break;
				}
			}
			if (entry == null)
			{
				entry = new DrawTextEntry();
				_textEntries.Add(entry);
			}

			entry.occupied = true;
			entry.anchor = anchor;
			entry.content.text = text;
			entry.size = size;
			entry.color = color;
			entry.duration = entry.timer = timer;
			entry.popUp = popUp;
#if UNITY_EDITOR
			entry.flag = DrawTextEntry.DrawFlag.None;
#else
			//	in builds consider gizmo is already drawn
			entry.flag = DrawTextEntry.DrawFlag.DrawnGizmo;
#endif

			return;
		}

		private void TickTexts()
		{
			for (int ix = 0; ix < _textEntries.Count; ix++)
			{
				var entry = _textEntries[ix];
				if (!entry.occupied)
					continue;
				entry.timer -= Time.deltaTime;
				if (entry.timer < 0
					&& entry.flag == DrawTextEntry.DrawFlag.DrawnAll)
				{
					entry.occupied = false;
				}
			}

			return;
		}

		private void DrawTextOnGUI()
		{
			for (int ix = 0; ix < _textEntries.Count; ix++)
			{
				var entry = _textEntries[ix];
				if (!entry.occupied)
					continue;

				var camera = Draw.GetDebugDrawCamera();
				if (camera != null)
					GUIDrawTextEntry(camera, entry);

				entry.flag |= DrawTextEntry.DrawFlag.DrawnGUI;
			}

			return;
		}

		private void GUIDrawTextEntry(Camera camera, DrawTextEntry entry)
		{
			Vector3 worldPos = entry.anchor;
			Vector3 screenPos = camera.WorldToScreenPoint(worldPos);
			screenPos.y = Screen.height - screenPos.y;

			if (entry.popUp)
			{
				float ratio = entry.timer / entry.duration;
				screenPos.y -=  (1 - ratio * ratio) * entry.size * 1.5f;
			}

			_textStyle.normal.textColor = entry.color;
			_textStyle.fontSize = entry.size;
			Rect rect = new Rect(screenPos, _textStyle.CalcSize(entry.content));
			GUI.Label(rect, entry.content, _textStyle);

			return;
		}

#if UNITY_EDITOR
		private void DrawTextOnDrawGizmos()
		{
			if (!(Camera.current == Draw.GetDebugDrawCamera()
				|| Camera.current == UnityEditor.SceneView.lastActiveSceneView.camera))
				return;

			UnityEditor.Handles.BeginGUI();
			for (int ix = 0; ix < _textEntries.Count; ix++)
			{
				var entry = _textEntries[ix];
				if (!entry.occupied)
					continue;

				GUIDrawTextEntry(Camera.current, entry);
				entry.flag |= DrawTextEntry.DrawFlag.DrawnGizmo;
			}
			UnityEditor.Handles.EndGUI();

			return;
		}
#endif
		#endregion
	}
}

