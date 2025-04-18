using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System;

[Serializable]
public class TodoItem
{
    public bool isComplete;
    public string text;
    public string listName; // Which list this task belongs to
}

[Serializable]
public class TodoList
{
    public string name;
    public bool isExpanded = true;
}

[Serializable]
public class NotePage
{
    public string pageName = "New Page";
    public string content = "";
}

[Serializable]
public class NotepadData
{
    public List<NotePage> notePages = new List<NotePage>();
    public List<TodoItem> todoItems = new List<TodoItem>();
    public List<TodoList> todoLists = new List<TodoList>();
    public int activePageIndex = 0;
}

public class EditorNotepadWithTodo : EditorWindow
{
    // Data container for all content
    private NotepadData data = new NotepadData();
    
    // Scroll positions for both panels
    private Vector2 noteScrollPosition;
    private Vector2 todoScrollPosition;
    private Vector2 pageScrollPosition;
    private Vector2 listScrollPosition;
    
    // File path for saving/loading
    private string dataFilePath;
    
    // UI state tracking
    private string newTodoText = "";
    private string newPageName = "";
    private string newListName = "";
    private bool showCompletedTasks = true;
    private bool showPagesList = true;
    private bool showTodoLists = true;
    
    // Currently selected list for adding new tasks
    private int selectedListIndex = -1; // -1 means "All Tasks"
    
    // Tab selection
    private int selectedTab = 0;
    private readonly string[] tabOptions = new string[] { "Notes", "To-Do List" };
    
    // Add menu item to create the window
    [MenuItem("Window/Developer Notepad")]
    public static void ShowWindow()
    {
        // Get existing open window or create a new one
        EditorNotepadWithTodo window = GetWindow<EditorNotepadWithTodo>("Dev Notepad");
        window.minSize = new Vector2(400, 500);
        window.Show();
    }
    
    private void OnEnable()
    {
        // Define the file path - saves in the project directory
        dataFilePath = Path.Combine(Application.dataPath, "EditorNotesAndTodos.json");
        
        // Load any existing data
        LoadData();
        
        // Create default page if none exists
        if (data.notePages.Count == 0)
        {
            data.notePages.Add(new NotePage { pageName = "General Notes", content = "" });
        }
        
        // Create default lists if none exist
        if (data.todoLists.Count == 0)
        {
            data.todoLists.Add(new TodoList { name = "General" });
            selectedListIndex = 0;
        }
    }
    
    private void OnGUI()
    {
        EditorGUILayout.BeginVertical();
        
        // Common controls at the top
        EditorGUILayout.BeginHorizontal();
        
        if (GUILayout.Button("Save All", GUILayout.Width(80)))
        {
            SaveData();
        }
        
        if (GUILayout.Button("Load", GUILayout.Width(80)))
        {
            LoadData();
        }
        
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.Space();
        
        // Tab selection
        selectedTab = GUILayout.Toolbar(selectedTab, tabOptions);
        
        EditorGUILayout.Space();
        
        // Content based on selected tab
        switch (selectedTab)
        {
            case 0: // Notes tab
                DrawNotes();
                break;
            case 1: // To-Do List tab
                DrawTodo();
                break;
        }
        
        EditorGUILayout.EndVertical();
        
        // Auto-save when Unity loses focus
        if (Event.current.type == EventType.MouseDown && !position.Contains(Event.current.mousePosition))
        {
            SaveData();
        }
    }
    
    private void DrawNotes()
    {
        EditorGUILayout.LabelField("Developer Notes", EditorStyles.boldLabel);
        
        // Pages management section
        EditorGUILayout.BeginHorizontal();
        
        showPagesList = EditorGUILayout.Foldout(showPagesList, "Pages", true);
        
        EditorGUILayout.EndHorizontal();
        
        if (showPagesList)
        {
            EditorGUI.indentLevel++;
            
            // Page selection list
            pageScrollPosition = EditorGUILayout.BeginScrollView(pageScrollPosition, GUILayout.Height(100));
            
            for (int i = 0; i < data.notePages.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                
                // Select this page
                if (GUILayout.Toggle(data.activePageIndex == i, "", GUILayout.Width(20)))
                {
                    data.activePageIndex = i;
                }
                
                // Edit page name
                data.notePages[i].pageName = EditorGUILayout.TextField(data.notePages[i].pageName);
                
                // Delete page button (prevent deleting if it's the last page)
                if (data.notePages.Count > 1 && GUILayout.Button("×", GUILayout.Width(25)))
                {
                    if (EditorUtility.DisplayDialog("Delete Page", 
                        "Are you sure you want to delete the page '" + data.notePages[i].pageName + "'?", 
                        "Delete", "Cancel"))
                    {
                        data.notePages.RemoveAt(i);
                        if (data.activePageIndex >= data.notePages.Count)
                        {
                            data.activePageIndex = data.notePages.Count - 1;
                        }
                    }
                }
                
                EditorGUILayout.EndHorizontal();
            }
            
            EditorGUILayout.EndScrollView();
            
            // New page creation
            EditorGUILayout.BeginHorizontal();
            newPageName = EditorGUILayout.TextField("New Page:", newPageName);
            if (GUILayout.Button("Add", GUILayout.Width(60)) && !string.IsNullOrWhiteSpace(newPageName))
            {
                data.notePages.Add(new NotePage { pageName = newPageName, content = "" });
                data.activePageIndex = data.notePages.Count - 1;
                newPageName = "";
            }
            EditorGUILayout.EndHorizontal();
            
            EditorGUI.indentLevel--;
        }
        
        EditorGUILayout.Space();
        
        // Display current page title
        if (data.notePages.Count > 0 && data.activePageIndex >= 0 && data.activePageIndex < data.notePages.Count)
        {
            EditorGUILayout.LabelField(data.notePages[data.activePageIndex].pageName, EditorStyles.boldLabel);
            
            if (GUILayout.Button("Clear Current Page", GUILayout.Width(150)))
            {
                if (EditorUtility.DisplayDialog("Clear Page", 
                    "Are you sure you want to clear the content of this page?", 
                    "Clear", "Cancel"))
                {
                    data.notePages[data.activePageIndex].content = "";
                }
            }
            
            EditorGUILayout.Space();
            
            // Create a scroll view for the text area
            noteScrollPosition = EditorGUILayout.BeginScrollView(noteScrollPosition, GUILayout.ExpandHeight(true));
            
            // Text area for notes - allows for multiline input
            data.notePages[data.activePageIndex].content = EditorGUILayout.TextArea(
                data.notePages[data.activePageIndex].content, GUILayout.ExpandHeight(true));
            
            EditorGUILayout.EndScrollView();
        }
    }
    
    private void DrawTodo()
    {
        EditorGUILayout.LabelField("To-Do Lists", EditorStyles.boldLabel);
        
        // Lists management section
        EditorGUILayout.BeginHorizontal();
        
        showTodoLists = EditorGUILayout.Foldout(showTodoLists, "Lists", true);
        
        EditorGUILayout.EndHorizontal();
        
        if (showTodoLists)
        {
            EditorGUI.indentLevel++;
            
            // Display "All Tasks" option
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Toggle(selectedListIndex == -1, "", GUILayout.Width(20)))
            {
                selectedListIndex = -1;
            }
            EditorGUILayout.LabelField("All Tasks");
            EditorGUILayout.EndHorizontal();
            
            // List selection list
            listScrollPosition = EditorGUILayout.BeginScrollView(listScrollPosition, GUILayout.Height(100));
            
            List<int> listsToRemove = new List<int>();
            
            for (int i = 0; i < data.todoLists.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                
                // Select this list
                if (GUILayout.Toggle(selectedListIndex == i, "", GUILayout.Width(20)))
                {
                    selectedListIndex = i;
                }
                
                // Edit list name
                data.todoLists[i].name = EditorGUILayout.TextField(data.todoLists[i].name);
                
                // Delete list button
                if (GUILayout.Button("×", GUILayout.Width(25)))
                {
                    if (EditorUtility.DisplayDialog("Delete List", 
                        "Are you sure you want to delete the list '" + data.todoLists[i].name + "' and all its tasks?", 
                        "Delete", "Cancel"))
                    {
                        // Mark tasks in this list for deletion
                        for (int j = data.todoItems.Count - 1; j >= 0; j--)
                        {
                            if (data.todoItems[j].listName == data.todoLists[i].name)
                            {
                                data.todoItems.RemoveAt(j);
                            }
                        }
                        
                        listsToRemove.Add(i);
                    }
                }
                
                EditorGUILayout.EndHorizontal();
            }
            
            // Remove marked lists
            for (int i = listsToRemove.Count - 1; i >= 0; i--)
            {
                int index = listsToRemove[i];
                data.todoLists.RemoveAt(index);
                
                // Update selected list index if necessary
                if (selectedListIndex == index)
                {
                    selectedListIndex = -1; // Default to "All Tasks"
                }
                else if (selectedListIndex > index)
                {
                    selectedListIndex--;
                }
            }
            
            EditorGUILayout.EndScrollView();
            
            // New list creation
            EditorGUILayout.BeginHorizontal();
            newListName = EditorGUILayout.TextField("New List:", newListName);
            if (GUILayout.Button("Add", GUILayout.Width(60)) && !string.IsNullOrWhiteSpace(newListName))
            {
                // Check if list name already exists
                bool listExists = false;
                foreach (var list in data.todoLists)
                {
                    if (list.name.Equals(newListName, StringComparison.OrdinalIgnoreCase))
                    {
                        listExists = true;
                        break;
                    }
                }
                
                if (!listExists)
                {
                    data.todoLists.Add(new TodoList { name = newListName });
                    selectedListIndex = data.todoLists.Count - 1;
                    newListName = "";
                }
                else
                {
                    EditorUtility.DisplayDialog("Duplicate List", 
                        "A list with that name already exists.", "OK");
                }
            }
            EditorGUILayout.EndHorizontal();
            
            EditorGUI.indentLevel--;
            
            EditorGUILayout.Space();
        }
        
        // Display current list title or "All Tasks"
        string currentListTitle = (selectedListIndex >= 0 && selectedListIndex < data.todoLists.Count) 
            ? data.todoLists[selectedListIndex].name + " Tasks" 
            : "All Tasks";
        
        EditorGUILayout.LabelField(currentListTitle, EditorStyles.boldLabel);
        
        // Filter controls
        EditorGUILayout.BeginHorizontal();
        
        showCompletedTasks = EditorGUILayout.Toggle("Show Completed", showCompletedTasks);
        
        if (GUILayout.Button("Clear Completed", GUILayout.Width(120)))
        {
            if (EditorUtility.DisplayDialog("Clear Completed", 
                "Are you sure you want to clear all completed tasks" + 
                (selectedListIndex >= 0 ? " in this list?" : "?"), 
                "Clear", "Cancel"))
            {
                if (selectedListIndex >= 0 && selectedListIndex < data.todoLists.Count)
                {
                    // Clear completed tasks from the current list only
                    string listName = data.todoLists[selectedListIndex].name;
                    data.todoItems.RemoveAll(item => item.isComplete && item.listName == listName);
                }
                else
                {
                    // Clear all completed tasks
                    data.todoItems.RemoveAll(item => item.isComplete);
                }
            }
        }
        
        EditorGUILayout.EndHorizontal();
        
        // Add new task field
        EditorGUILayout.BeginHorizontal();
        
        newTodoText = EditorGUILayout.TextField("New Task:", newTodoText);
        
        if (GUILayout.Button("Add", GUILayout.Width(60)) && !string.IsNullOrWhiteSpace(newTodoText))
        {
            string listName = (selectedListIndex >= 0 && selectedListIndex < data.todoLists.Count) 
                ? data.todoLists[selectedListIndex].name 
                : data.todoLists[0].name; // Default to first list if no list selected or "All Tasks" selected
            
            data.todoItems.Add(new TodoItem { isComplete = false, text = newTodoText, listName = listName });
            newTodoText = "";
            GUI.FocusControl(null); // Clear focus
        }
        
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.Space();
        
        // Todo items list
        todoScrollPosition = EditorGUILayout.BeginScrollView(todoScrollPosition, GUILayout.ExpandHeight(true));
        
        List<int> toRemove = new List<int>();
        
        // First, group tasks by list if showing all tasks
        Dictionary<string, List<TodoItem>> groupedItems = new Dictionary<string, List<TodoItem>>();
        
        if (selectedListIndex == -1)
        {
            // Show all lists grouped
            foreach (var list in data.todoLists)
            {
                groupedItems[list.name] = new List<TodoItem>();
            }
            
            // Group items by list
            foreach (var item in data.todoItems)
            {
                if (!groupedItems.ContainsKey(item.listName))
                {
                    groupedItems[item.listName] = new List<TodoItem>();
                }
                
                // Skip completed items if filter is active
                if (item.isComplete && !showCompletedTasks)
                    continue;
                    
                groupedItems[item.listName].Add(item);
            }
            
            // Display groups
            foreach (var group in groupedItems)
            {
                if (group.Value.Count > 0)
                {
                    EditorGUILayout.LabelField(group.Key, EditorStyles.boldLabel);
                    DrawTodoItems(group.Value, toRemove);
                    EditorGUILayout.Space();
                }
            }
        }
        else if (selectedListIndex >= 0 && selectedListIndex < data.todoLists.Count)
        {
            // Show only the selected list
            string listName = data.todoLists[selectedListIndex].name;
            List<TodoItem> filteredItems = new List<TodoItem>();
            
            for (int i = 0; i < data.todoItems.Count; i++)
            {
                if (data.todoItems[i].listName == listName)
                {
                    // Skip completed items if filter is active
                    if (data.todoItems[i].isComplete && !showCompletedTasks)
                        continue;
                        
                    filteredItems.Add(data.todoItems[i]);
                }
            }
            
            DrawTodoItems(filteredItems, toRemove);
        }
        
        EditorGUILayout.EndScrollView();
        
        // Remove marked items (do this after iteration to avoid issues)
        for (int i = toRemove.Count - 1; i >= 0; i--)
        {
            data.todoItems.RemoveAt(toRemove[i]);
        }
    }
    
    private void DrawTodoItems(List<TodoItem> items, List<int> toRemove)
    {
        for (int i = 0; i < data.todoItems.Count; i++)
        {
            TodoItem item = data.todoItems[i];
            
            // Skip if item doesn't belong to the items list we're drawing
            if (!items.Contains(item))
                continue;
                
            EditorGUILayout.BeginHorizontal();
            
            // Checkbox for completion status
            bool oldState = item.isComplete;
            item.isComplete = EditorGUILayout.Toggle(item.isComplete, GUILayout.Width(20));
            
            // Apply styling based on completion status
            GUIStyle style = new GUIStyle(EditorStyles.textField);
            if (item.isComplete)
            {
                style.normal.textColor = new Color(0.5f, 0.5f, 0.5f); // Grayed out text
                style.fontStyle = FontStyle.Italic;
            }
            
            // Task text
            item.text = EditorGUILayout.TextField(item.text, style);
            
            // Delete button
            if (GUILayout.Button("×", GUILayout.Width(25)))
            {
                toRemove.Add(i);
            }
            
            EditorGUILayout.EndHorizontal();
        }
    }
    
    private void SaveData()
    {
        try
        {
            // Convert data to JSON
            string jsonData = JsonUtility.ToJson(data, true);
            File.WriteAllText(dataFilePath, jsonData);
            Debug.Log("Notes and todos saved to: " + dataFilePath);
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to save data: " + e.Message);
        }
    }
    
    private void LoadData()
    {
        if (File.Exists(dataFilePath))
        {
            try
            {
                string jsonData = File.ReadAllText(dataFilePath);
                NotepadData loadedData = JsonUtility.FromJson<NotepadData>(jsonData);
                
                if (loadedData != null)
                {
                    data = loadedData;
                    
                    // Make sure we have valid pages
                    if (data.notePages == null || data.notePages.Count == 0)
                    {
                        data.notePages = new List<NotePage> { new NotePage { pageName = "General Notes", content = "" } };
                        data.activePageIndex = 0;
                    }
                    
                    // Make sure todo list is initialized
                    if (data.todoItems == null)
                    {
                        data.todoItems = new List<TodoItem>();
                    }
                    
                    // Make sure todo lists are initialized
                    if (data.todoLists == null || data.todoLists.Count == 0)
                    {
                        data.todoLists = new List<TodoList> { new TodoList { name = "General" } };
                        selectedListIndex = 0;
                    }
                    
                    // Ensure all tasks have a valid list (in case of data corruption)
                    foreach (var item in data.todoItems)
                    {
                        if (string.IsNullOrEmpty(item.listName) || 
                            !data.todoLists.Exists(l => l.name == item.listName))
                        {
                            item.listName = data.todoLists[0].name;
                        }
                    }
                    
                    // Make sure active page index is valid
                    if (data.activePageIndex < 0 || data.activePageIndex >= data.notePages.Count)
                    {
                        data.activePageIndex = 0;
                    }
                }
                else
                {
                    CreateDefaultData();
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Failed to load data: " + e.Message);
                CreateDefaultData();
            }
        }
        else
        {
            CreateDefaultData();
        }
    }
    
    private void CreateDefaultData()
    {
        data = new NotepadData
        {
            notePages = new List<NotePage> { new NotePage { pageName = "General Notes", content = "" } },
            todoItems = new List<TodoItem>(),
            todoLists = new List<TodoList> { new TodoList { name = "General" } },
            activePageIndex = 0
        };
        selectedListIndex = 0;
    }
    
    // Auto-save when the window is closed
    private void OnDestroy()
    {
        SaveData();
    }
}