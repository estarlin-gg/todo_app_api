using Todo_api.Models;

namespace Todo_api.Delegates;

public delegate bool TaskValidator(TodoTask<string> task);
public delegate void TaskNotifier(string message);
public delegate int TaskDueDateCalculator(DateTime dueDate);
