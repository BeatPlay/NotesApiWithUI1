using Microsoft.AspNetCore.Mvc;
using NotesApiWithUI.Models;
using NotesApiWithUI.Services;

namespace NotesApiWithUI.Controllers
{
    public class HomeController : Controller
    {
        private readonly NoteService _noteService;
        private readonly TelegramService _telegramService;

        public HomeController(NoteService noteService, TelegramService telegramService)
        {
            _noteService = noteService;
            _telegramService = telegramService;
        }

        public IActionResult Index()
        {
            var notes = _noteService.GetAll();
            return View(notes);
        }

        [HttpPost]
        public async Task<IActionResult> Add(Note note)
        {
            if (!ModelState.IsValid)
                return RedirectToAction("Index");

            var addedNote = _noteService.Add(note);

            // Отправка уведомления в Telegram
            string message = $"Добавлена новая заметка:\n\n" +
                                 $"ID: {addedNote.Id}\n" +
                                 $"Заголовок: {addedNote.Title}\n" +
                                 $"Содержимое: {addedNote.Content}";
            await _telegramService.SendMessageAsync(message);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int id)
        {
            var deleted = _noteService.Delete(id);

            if (deleted)
            {
                // Отправка уведомления в Telegram
                await _telegramService.SendMessageAsync($"Заметка с ID {id} была удалена.");
            }

            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            var note = _noteService.GetById(id);
            if (note == null) return NotFound();

            return View(note);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Note updatedNote)
        {
            if (!ModelState.IsValid)
                return View(updatedNote);

            var note = _noteService.Update(updatedNote.Id, updatedNote);
            if (note == null) return NotFound();

            // Отправка уведомления в Telegram
            string message = $"Обновлена заметка:\n\n" +
                                 $"ID: {updatedNote.Id}\n" +
                                 $"Заголовок: {updatedNote.Title}\n" +
                                 $"Новое содержимое: {updatedNote.Content}";
            await _telegramService.SendMessageAsync(message);

            return RedirectToAction("Index");
        }

    }
}