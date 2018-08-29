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
			_rtDraw.RegisterDrawText(pos, text, color, size, duration, popUp);
			return;
		}

		/// <summary>
		/// Attach text to a transform.
		/// </summary>
		/// <param name="transform">Target transform to attach text to.</param>
		/// <param name="strFunc">Function will be called on every frame to get a string as attached text. </param>
		/// <param name="offset">Text attach offset to transform position.</param>
		/// <param name="color">Color for the text.</param>
		/// <param name="size">Font size for the text.</param>
		[Conditional("_DEBUG")]
		public static void AttachText(Transform transform, Func<string> strFunc, Vector3 offset, Color color, int size)
		{
			CheckAndBuildHiddenRTDrawObject();
			_rtDraw.RegisterAttachText(transform, strFunc, offset, color, size);

			return;
		}

		/// <summary>
		/// Draw a bitmap, ASCII only text at given position.
		/// </summary>
		/// <param name="pos">Position</param>
		/// <param name="text">String of the text.</param>
		/// <param name="color">Color for the text.</param>
		/// <param name="size">Font size for the text.</param>
		/// <param name="duration">How long the text should be visible for.</param>
		/// <param name="popUp">Set to true to let the text moving up, so multiple texts at the same position can be visible.</param>
		[Conditional("_DEBUG")]
		public static void DrawBitmapText(Vector3 pos, string text, Color color, int size, float duration, bool popUp)
		{
			CheckAndBuildHiddenRTDrawObject();
			_rtDraw.RegisterDrawBitmapText(pos, text, color, size, duration, popUp);
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

		/// <summary>
		/// Attach text to a transform.
		/// </summary>
		/// <param name="transform">Target transform to attach text to.</param>
		/// <param name="strFunc">Function will be called on every frame to get a string as attached text. </param>
		[Conditional("_DEBUG")]
		public static void AttachText(Transform transform, Func<string> strFunc)
		{
			AttachText(transform, strFunc, Vector3.zero, DrawDefaultColor, DrawTextDefaultSize);
			return;
		}

		/// <summary>
		/// Attach text to a transform.
		/// </summary>
		/// <param name="transform">Target transform to attach text to.</param>
		/// <param name="strFunc">Function will be called on every frame to get a string as attached text. </param>
		/// <param name="offset">Text attach offset to transform position.</param>
		[Conditional("_DEBUG")]
		public static void AttachText(Transform transform, Func<string> strFunc, Vector3 offset)
		{
			AttachText(transform, strFunc, offset, DrawDefaultColor, DrawTextDefaultSize);
			return;
		}

		/// <summary>
		/// Attach text to a transform.
		/// </summary>
		/// <param name="transform">Target transform to attach text to.</param>
		/// <param name="strFunc">Function will be called on every frame to get a string as attached text. </param>
		/// <param name="offset">Text attach offset to transform position.</param>
		/// <param name="color">Color for the text.</param>
		[Conditional("_DEBUG")]
		public static void AttachText(Transform transform, Func<string> strFunc, Vector3 offset, Color color)
		{
			AttachText(transform, strFunc, offset, color, DrawTextDefaultSize);
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
		private static string HIDDEN_GO_NAME = "________HIDDEN_C4F6A87F298241078E21C0D7C1D87A76_";
		private static void CheckAndBuildHiddenRTDrawObject()
		{
			if (_rtDraw != null)
				return;

			//	try reuse existing one first
			_rtDraw = GameObject.FindObjectOfType<RuntimeDebugDraw.Internal.RuntimeDebugDraw>();
			if (_rtDraw != null)
				return;

			//	instantiate an hidden gameobject w/ RuntimeDebugDraw attached.
			//	hardcode an GUID in the name so one won't accidentally get this by name.
			var go = new GameObject(HIDDEN_GO_NAME);
			var childGo = new GameObject(HIDDEN_GO_NAME);
			childGo.transform.parent = go.transform;
			_rtDraw = childGo.AddComponent<RuntimeDebugDraw.Internal.RuntimeDebugDraw>();
			//	hack to only hide outer go, so that RuntimeDebugDraw's OnGizmos will work properly.
#if UNITY_2017_1_OR_NEWER
			//  !   removed Don'tSave flag as it'll throw weird error in 2017 and onwards
			//      however makes it potentially 
			go.hideFlags = HideFlags.HideInHierarchy | HideFlags.HideInInspector;
#else
			go.hideFlags = HideFlags.HideAndDontSave;
#endif

			if (Application.isPlaying)
				GameObject.DontDestroyOnLoad(go);

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
		private void CheckInitialized()
		{
			//	as RuntimeDebugDraw component has a very low execution order, other script might Awake()
			//	earlier than this and at that moment it's not initialized. check and init on every public
			//	member
			if (_drawTextEntries == null)
			{
				_ZTestBatch = new BatchedLineDraw(depthTest: true);
				_AlwaysBatch = new BatchedLineDraw(depthTest: false);
				_lineEntries = new List<DrawLineEntry>(16);

				_textStyle = new GUIStyle();
				_textStyle.alignment = TextAnchor.UpperLeft;
				_drawTextEntries = new List<DrawTextEntry>(16);
				_attachTextEntries = new List<AttachTextEntry>(16);

				_drawBitmapTextEntries = new List<DrawBitmapTextEntry>();
				_bitmapTextDrawBatch = new BatchedBitmapFontDraw();
			}

			return;
		}

		private void Awake()
		{
			CheckInitialized();

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

		private void LateUpdate()
		{
			TickAndDrawLines();
			TickTexts();
			TickAndDrawBitmapTexts();

			return;
		}

		private void OnDestroy()
		{
			_AlwaysBatch.Dispose();
			_ZTestBatch.Dispose();
			_bitmapTextDrawBatch.Dispose();

			return;
		}

		private void Clear()
		{
			_drawTextEntries.Clear();
			_lineEntries.Clear();
			_linesNeedRebuild = true;

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
					? 4 // LEqual
					: 0 // Always
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
				mesh.SetIndices(_indices.ToArray(), MeshTopology.Lines, 0); // cant get rid of this alloc for now

				return;
			}

			public void Dispose()
			{
				GameObject.DestroyImmediate(mesh);
				GameObject.DestroyImmediate(mat);

				return;
			}
		}

		private BatchedLineDraw _ZTestBatch;
		private BatchedLineDraw _AlwaysBatch;
		private bool _linesNeedRebuild;

		public void RegisterLine(Vector3 start, Vector3 end, Color color, float timer, bool noZTest)
		{
			CheckInitialized();

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
			Graphics.DrawMesh(_AlwaysBatch.mesh, Vector3.zero, Quaternion.identity, _AlwaysBatch.mat, layer: Draw.DrawLineLayer, camera: null, submeshIndex: 0, properties: null, castShadows: false, receiveShadows: false);
			Graphics.DrawMesh(_ZTestBatch.mesh, Vector3.zero, Quaternion.identity, _ZTestBatch.mat, layer: Draw.DrawLineLayer, camera: null, submeshIndex: 0, properties: null, castShadows: false, receiveShadows: false);

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
		[Flags]
		public enum DrawFlag : byte
		{
			None = 0,
			DrawnGizmo = 1 << 0,
			DrawnGUI = 1 << 1,
			DrawnAll = DrawnGizmo | DrawnGUI
		}

		private abstract class BaseDrawTextEntry
		{
			public bool occupied;

			//	Text entries needs to be draw in both OnGUI/OnDrawGizmos, need flags for mark
			//	has been visited by both
			public DrawFlag flag = DrawFlag.None;
		}

		private class DrawTextEntry : BaseDrawTextEntry
		{
			public GUIContent content;
			public Vector3 anchor;
			public int size;
			public Color color;
			public float timer;
			public bool popUp;
			public float duration;

			public DrawTextEntry()
			{
				content = new GUIContent();
				return;
			}
		}

		private class AttachTextEntry : BaseDrawTextEntry
		{
			public GUIContent content;
			public Vector3 offset;
			public int size;
			public Color color;


			public Transform transform;
			public Func<string> strFunc;

			public AttachTextEntry()
			{
				content = new GUIContent();
				return;
			}
		}

		private List<DrawTextEntry> _drawTextEntries;
		private List<AttachTextEntry> _attachTextEntries;
		private GUIStyle _textStyle;

		private T SeekFirstUnoccupiedEntryOrCreate<T>(List<T> entries) where T: BaseDrawTextEntry, new()
		{
			T entry = null;
			for (int ix = 0; ix < entries.Count; ix++)
			{
				if (!entries[ix].occupied)
				{
					entry = entries[ix];
					break;
				}
			}
			if (entry == null)
			{
				entry = new T();
				entries.Add(entry);
			}

			return entry;
		}

		public void RegisterDrawText(Vector3 anchor, string text, Color color, int size, float timer, bool popUp)
		{
			CheckInitialized();

			DrawTextEntry entry = SeekFirstUnoccupiedEntryOrCreate(_drawTextEntries);

			entry.occupied = true;
			entry.anchor = anchor;
			entry.content.text = text;
			entry.size = size;
			entry.color = color;
			entry.duration = entry.timer = timer;
			entry.popUp = popUp;
#if UNITY_EDITOR
			entry.flag = DrawFlag.None;
#else
			//	in builds consider gizmo is already drawn
			entry.flag = DrawFlag.DrawnGizmo;
#endif

			return;
		}

		public void RegisterAttachText(Transform target, Func<string> strFunc, Vector3 offset, Color color, int size)
		{
			CheckInitialized();

			AttachTextEntry entry = SeekFirstUnoccupiedEntryOrCreate(_attachTextEntries);

			entry.occupied = true;
			entry.offset = offset;
			entry.transform = target;
			entry.strFunc = strFunc;
			entry.color = color;
			entry.size = size;
			//	get first text
			entry.content.text = strFunc();
#if UNITY_EDITOR
			entry.flag = DrawFlag.None;
#else
			//	in builds consider gizmo is already drawn
			entry.flag = DrawFlag.DrawnGizmo;
#endif

			return;
		}

		private void TickTexts()
		{
			for (int ix = 0; ix < _drawTextEntries.Count; ix++)
			{
				var entry = _drawTextEntries[ix];
				if (!entry.occupied)
					continue;
				entry.timer -= Time.deltaTime;
				if (entry.flag == DrawFlag.DrawnAll)
				{
					if (entry.timer < 0)
					{
						entry.occupied = false;
					}
					//	actually no need to tick DrawFlag as it won't move
				}
			}

			for (int ix = 0; ix < _attachTextEntries.Count; ix++)
			{
				var entry = _attachTextEntries[ix];
				if (!entry.occupied)
					continue;
				if (entry.transform == null)
				{
					entry.occupied = false;
					entry.strFunc = null;   // needs to release ref to callback
				}
				else if (entry.flag == DrawFlag.DrawnAll)
				{
					// tick content
					entry.content.text = entry.strFunc();
					// tick flag
#if UNITY_EDITOR
					entry.flag = DrawFlag.None;
#else
					//	in builds consider gizmo is already drawn
					entry.flag = DrawFlag.DrawnGizmo;
#endif
				}
			}
		}


		private void RebuildBitmapTextBatchMesh()
		{
			_bitmapTextDrawBatch.Clear();

			for (int ix = 0; ix < _drawBitmapTextEntries.Count; ix++)
			{
				var entry = _drawBitmapTextEntries[ix];
				if (!entry.occupied)
					continue;

				_bitmapTextDrawBatch.AddBitmapText(entry.anchor.x, entry.anchor.y, entry.text, entry.color);
			}

			_bitmapTextDrawBatch.BuildBatch();

			return;
		}

		private void TickAndDrawBitmapTexts()
		{
			// TODO dirty check
			RebuildBitmapTextBatchMesh();

			//	draw on UI layer which should bypass most postFX setups
			Graphics.DrawMesh(_bitmapTextDrawBatch.mesh, Vector3.zero, Quaternion.identity, _bitmapTextDrawBatch.mat, layer: Draw.DrawLineLayer, camera: null, submeshIndex: 0, properties: null, castShadows: false, receiveShadows: false);

			for (int ix = 0; ix < _drawBitmapTextEntries.Count; ix++)
			{
				var entry = _drawBitmapTextEntries[ix];
				if (!entry.occupied)
					continue;
				entry.timer -= Time.deltaTime;
				if (entry.flag == DrawFlag.DrawnAll)
				{
					if (entry.timer < 0)
					{
						entry.occupied = false;
					}
					//	actually no need to tick DrawFlag as it won't move
				}
			}


			return;
		}

		private void DrawTextOnGUI()
		{
			var camera = Draw.GetDebugDrawCamera();
			if (camera == null)
				return;

			for (int ix = 0; ix < _drawTextEntries.Count; ix++)
			{
				var entry = _drawTextEntries[ix];
				if (!entry.occupied)
					continue;

				GUIDrawTextEntry(camera, entry);
				entry.flag |= DrawFlag.DrawnGUI;
			}

			for (int ix = 0; ix < _attachTextEntries.Count; ix++)
			{
				var entry = _attachTextEntries[ix];
				if (!entry.occupied)
					continue;

				GUIAttachTextEntry(camera, entry);
				entry.flag |= DrawFlag.DrawnGUI;
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
				screenPos.y -= (1 - ratio * ratio) * entry.size * 1.5f;
			}

			_textStyle.normal.textColor = entry.color;
			_textStyle.fontSize = entry.size;
			Rect rect = new Rect(screenPos, _textStyle.CalcSize(entry.content));
			GUI.Label(rect, entry.content, _textStyle);

			return;
		}

		private void GUIAttachTextEntry(Camera camera, AttachTextEntry entry)
		{
			if (entry.transform == null)
				return;

			Vector3 worldPos = entry.transform.position + entry.offset;
			Vector3 screenPos = camera.WorldToScreenPoint(worldPos);
			screenPos.y = Screen.height - screenPos.y;

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

			var camera = Camera.current;
			if (camera == null)
				return;

			UnityEditor.Handles.BeginGUI();
			for (int ix = 0; ix < _drawTextEntries.Count; ix++)
			{
				var entry = _drawTextEntries[ix];
				if (!entry.occupied)
					continue;

				GUIDrawTextEntry(camera, entry);
				entry.flag |= DrawFlag.DrawnGizmo;
			}

			for (int ix = 0; ix < _attachTextEntries.Count; ix++)
			{
				var entry = _attachTextEntries[ix];
				if (!entry.occupied)
					continue;

				GUIAttachTextEntry(camera, entry);
				entry.flag |= DrawFlag.DrawnGizmo;
			}

			UnityEditor.Handles.EndGUI();

			return;
		}
#endif
		#endregion

#region Draw Bitmap Text
		//	helper class for bitmap font batching
		private class BatchedBitmapFontDraw : IDisposable
		{
			public Mesh mesh;
			public Material mat;

			private List<Vector3> _vertices;
			private List<Color32> _colors;
			private List<int> _indices;

			public BatchedBitmapFontDraw()
			{
				mesh = new Mesh();
				mesh.MarkDynamic();

				//	relying on a builtin shader, but it shouldn't change that much.
				mat = new Material(Shader.Find("Hidden/Internal-Colored"));
				mat.SetInt("_ZTest", 0);	// 'Always' for bitmap font

				_vertices = new List<Vector3>();
				_colors = new List<Color32>();
				_indices = new List<int>();

				return;
			}

			public void AddBitmapText(float x, float y, string text, Color color)
			{
				int count = StbEasyFont.GenerateMesh(x, y, text, color, _vertices, _colors);
				int start = _indices.Count;
				int end = start + count;
				for (int ix = start; ix < end; ix++)
					_indices.Add(ix);

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
				mesh.SetIndices(_indices.ToArray(), MeshTopology.Lines, 0); // cant get rid of this alloc for now

				return;
			}

			public void Dispose()
			{
				GameObject.DestroyImmediate(mesh);
				GameObject.DestroyImmediate(mat);

				return;
			}
		}

		private class DrawBitmapTextEntry : BaseDrawTextEntry
		{
			public string text;
			public Vector3 anchor;
			public int size;
			public Color color;
			public float timer;
			public bool popUp;
			public float duration;
		}

		private List<DrawBitmapTextEntry> _drawBitmapTextEntries;
		private BatchedBitmapFontDraw _bitmapTextDrawBatch;

		public void RegisterDrawBitmapText(Vector3 anchor, string text, Color color, int size, float timer, bool popUp)
		{
			CheckInitialized();

			DrawBitmapTextEntry entry = SeekFirstUnoccupiedEntryOrCreate(_drawBitmapTextEntries);

			entry.occupied = true;
			entry.anchor = anchor;
			entry.text = text;
			entry.size = size;
			entry.color = color;
			entry.duration = entry.timer = timer;
			entry.popUp = popUp;
#if UNITY_EDITOR
			entry.flag = DrawFlag.None;
#else
			//	in builds consider gizmo is already drawn
			entry.flag = DrawFlag.DrawnGizmo;
#endif

			return;
		}

#endregion

		#region StbEasyFont
		// A copy of https://github.com/aras-p/UnityStbEasyFont/blob/master/Assets/UnityStbEasyFont/StbEasyFont.cs
		// Moved into internal namespace to avoid clashing
		//  
		// port of stb_easy_font.h into Unity/C# - public domain
		// Aras Pranckevicius, 2015 November
		// https://github.com/aras-p/UnityStbEasyFont
		//
		// Original one: https://github.com/nothings/stb/blob/master/stb_easy_font.h
		// stb_easy_font.h - v0.6 - bitmap font for 3D rendering - public domain
		// Sean Barrett, Feb 2015
		public class StbEasyFont
		{
			struct CharInfo
			{
				public CharInfo(byte a, byte h, byte v)
				{
					advance = a;
					h_seg = h;
					v_seg = v;
				}
				public byte advance;
				public byte h_seg;
				public byte v_seg;
			};
			static CharInfo[] kCharInfo = new CharInfo[96]
			{
			new CharInfo(  5,  0,  0 ),  new CharInfo(  3,  0,  0 ),  new CharInfo(  5,  1,  1 ),  new CharInfo(  7,  1,  4 ),
			new CharInfo(  7,  3,  7 ),  new CharInfo(  7,  6, 12 ),  new CharInfo(  7,  8, 19 ),  new CharInfo(  4, 16, 21 ),
			new CharInfo(  4, 17, 22 ),  new CharInfo(  4, 19, 23 ),  new CharInfo( 23, 21, 24 ),  new CharInfo( 23, 22, 31 ),
			new CharInfo( 20, 23, 34 ),  new CharInfo( 22, 23, 36 ),  new CharInfo( 19, 24, 36 ),  new CharInfo( 21, 25, 36 ),
			new CharInfo(  6, 25, 39 ),  new CharInfo(  6, 27, 43 ),  new CharInfo(  6, 28, 45 ),  new CharInfo(  6, 30, 49 ),
			new CharInfo(  6, 33, 53 ),  new CharInfo(  6, 34, 57 ),  new CharInfo(  6, 40, 58 ),  new CharInfo(  6, 46, 59 ),
			new CharInfo(  6, 47, 62 ),  new CharInfo(  6, 55, 64 ),  new CharInfo( 19, 57, 68 ),  new CharInfo( 20, 59, 68 ),
			new CharInfo( 21, 61, 69 ),  new CharInfo( 22, 66, 69 ),  new CharInfo( 21, 68, 69 ),  new CharInfo(  7, 73, 69 ),
			new CharInfo(  9, 75, 74 ),  new CharInfo(  6, 78, 81 ),  new CharInfo(  6, 80, 85 ),  new CharInfo(  6, 83, 90 ),
			new CharInfo(  6, 85, 91 ),  new CharInfo(  6, 87, 95 ),  new CharInfo(  6, 90, 96 ),  new CharInfo(  7, 92, 97 ),
			new CharInfo(  6, 96,102 ),  new CharInfo(  5, 97,106 ),  new CharInfo(  6, 99,107 ),  new CharInfo(  6,100,110 ),
			new CharInfo(  6,100,115 ),  new CharInfo(  7,101,116 ),  new CharInfo(  6,101,121 ),  new CharInfo(  6,101,125 ),
			new CharInfo(  6,102,129 ),  new CharInfo(  7,103,133 ),  new CharInfo(  6,104,140 ),  new CharInfo(  6,105,145 ),
			new CharInfo(  7,107,149 ),  new CharInfo(  6,108,151 ),  new CharInfo(  7,109,155 ),  new CharInfo(  7,109,160 ),
			new CharInfo(  7,109,165 ),  new CharInfo(  7,118,167 ),  new CharInfo(  6,118,172 ),  new CharInfo(  4,120,176 ),
			new CharInfo(  6,122,177 ),  new CharInfo(  4,122,181 ),  new CharInfo( 23,124,182 ),  new CharInfo( 22,129,182 ),
			new CharInfo(  4,130,182 ),  new CharInfo( 22,131,183 ),  new CharInfo(  6,133,187 ),  new CharInfo( 22,135,191 ),
			new CharInfo(  6,137,192 ),  new CharInfo( 22,139,196 ),  new CharInfo(  5,144,197 ),  new CharInfo( 22,147,198 ),
			new CharInfo(  6,150,202 ),  new CharInfo( 19,151,206 ),  new CharInfo( 21,152,207 ),  new CharInfo(  6,155,209 ),
			new CharInfo(  3,160,210 ),  new CharInfo( 23,160,211 ),  new CharInfo( 22,164,216 ),  new CharInfo( 22,165,220 ),
			new CharInfo( 22,167,224 ),  new CharInfo( 22,169,228 ),  new CharInfo( 21,171,232 ),  new CharInfo( 21,173,233 ),
			new CharInfo(  5,178,233 ),  new CharInfo( 22,179,234 ),  new CharInfo( 23,180,238 ),  new CharInfo( 23,180,243 ),
			new CharInfo( 23,180,248 ),  new CharInfo( 22,189,248 ),  new CharInfo( 22,191,252 ),  new CharInfo(  5,196,252 ),
			new CharInfo(  3,203,252 ),  new CharInfo(  5,203,253 ),  new CharInfo( 22,210,253 ),  new CharInfo(  0,214,253 ),
			};
			static byte[] kHSegs = new byte[214]
			{
			97,37,69,84,28,51,2,18,10,49,98,41,65,25,81,105,33,9,97,1,97,37,37,36,
			81,10,98,107,3,100,3,99,58,51,4,99,58,8,73,81,10,50,98,8,73,81,4,10,50,
			98,8,25,33,65,81,10,50,17,65,97,25,33,25,49,9,65,20,68,1,65,25,49,41,
			11,105,13,101,76,10,50,10,50,98,11,99,10,98,11,50,99,11,50,11,99,8,57,
			58,3,99,99,107,10,10,11,10,99,11,5,100,41,65,57,41,65,9,17,81,97,3,107,
			9,97,1,97,33,25,9,25,41,100,41,26,82,42,98,27,83,42,98,26,51,82,8,41,
			35,8,10,26,82,114,42,1,114,8,9,73,57,81,41,97,18,8,8,25,26,26,82,26,82,
			26,82,41,25,33,82,26,49,73,35,90,17,81,41,65,57,41,65,25,81,90,114,20,
			84,73,57,41,49,25,33,65,81,9,97,1,97,25,33,65,81,57,33,25,41,25,
			};
			static byte[] kVSegs = new byte[253]
			{
			4,2,8,10,15,8,15,33,8,15,8,73,82,73,57,41,82,10,82,18,66,10,21,29,1,65,
			27,8,27,9,65,8,10,50,97,74,66,42,10,21,57,41,29,25,14,81,73,57,26,8,8,
			26,66,3,8,8,15,19,21,90,58,26,18,66,18,105,89,28,74,17,8,73,57,26,21,
			8,42,41,42,8,28,22,8,8,30,7,8,8,26,66,21,7,8,8,29,7,7,21,8,8,8,59,7,8,
			8,15,29,8,8,14,7,57,43,10,82,7,7,25,42,25,15,7,25,41,15,21,105,105,29,
			7,57,57,26,21,105,73,97,89,28,97,7,57,58,26,82,18,57,57,74,8,30,6,8,8,
			14,3,58,90,58,11,7,74,43,74,15,2,82,2,42,75,42,10,67,57,41,10,7,2,42,
			74,106,15,2,35,8,8,29,7,8,8,59,35,51,8,8,15,35,30,35,8,8,30,7,8,8,60,
			36,8,45,7,7,36,8,43,8,44,21,8,8,44,35,8,8,43,23,8,8,43,35,8,8,31,21,15,
			20,8,8,28,18,58,89,58,26,21,89,73,89,29,20,8,8,30,7,
			};

			//	! modified to return added vertex count
			static int GenerateSegs(float x, float y, byte[] segs, int segs_start, int num_segs, bool vertical, Color32 c, List<Vector3> vbuf, List<Color32> cbuf)
			{
				int count = 0;
				for (int i = segs_start; i < segs_start + num_segs; ++i)
				{
					int len = segs[i] & 7;
					x += (float)((segs[i] >> 3) & 1);
					if (len != 0)
					{
						float y0 = y + (float)(segs[i] >> 4);
						for (int j = 0; j < 4; ++j)
						{
							Vector3 pos = new Vector3(
								x + (j == 1 || j == 2 ? (vertical ? 1 : len) : 0),
								y0 + (j >= 2 ? (vertical ? len : 1) : 0),
								0.0f
							);
							vbuf.Add(pos);
							if (cbuf != null)
								cbuf.Add(c);
							count++;
						}
					}
				}

				return count;
			}

			//	! modified to return added vertex count
			static public int GenerateMesh(float x, float y, string text, Color32 color, List<Vector3> vertexBuffer, List<Color32> colorBuffer)
			{
				float start_x = x;
				int max_verts = 64000 / 4;

				int textIndex = 0;
				int textLength = text.Length;
				int count = 0;
				while (textIndex < textLength && vertexBuffer.Count < max_verts)
				{
					char textChar = text[textIndex];
					if (textChar == '\n' || textChar == '\t')
					{
						y += 12;
						x = start_x;
					}
					else if (textChar < ' ')
					{
						// just skip various other control chars
					}
					else
					{
						int charIndex = (int)textChar - 32;
						if (charIndex < 0 || charIndex >= kCharInfo.Length)
							charIndex = (int)'?' - 32;
						byte advance = kCharInfo[charIndex].advance;
						float y_ch = (advance & 16) != 0 ? y + 1 : y;
						int h_seg = kCharInfo[charIndex].h_seg;
						int v_seg = kCharInfo[charIndex].v_seg;
						int num_h = kCharInfo[charIndex + 1].h_seg - h_seg;
						int num_v = kCharInfo[charIndex + 1].v_seg - v_seg;
						count += GenerateSegs(x, y_ch, kHSegs, h_seg, num_h, false, color, vertexBuffer, colorBuffer);
						count += GenerateSegs(x, y_ch, kVSegs, v_seg, num_v, true, color, vertexBuffer, colorBuffer);
						x += advance & 15;
					}
					++textIndex;
				}

				return count;
			}
		}
#endregion
	}



}

