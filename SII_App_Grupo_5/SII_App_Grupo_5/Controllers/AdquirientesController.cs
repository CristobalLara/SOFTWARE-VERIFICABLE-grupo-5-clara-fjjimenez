using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SII_App_Grupo_5.Data;
using SII_App_Grupo_5.Models;

namespace SII_App_Grupo_5.Controllers
{
    public class AdquirientesController : Controller
    {
        private readonly InscripcionesGrupo5DbContext _context;

        public AdquirientesController(InscripcionesGrupo5DbContext context)
        {
            _context = context;
        }

        // GET: Adquirientes
        public async Task<IActionResult> Index()
        {
            var inscriptionsGrupo5DbContext = _context.Adquirientes.Include(a => a.Inscripcion);
            return View(await inscriptionsGrupo5DbContext.ToListAsync());
        }

        // GET: Adquirientes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Adquirientes == null)
            {
                return NotFound();
            }

            var adquiriente = await _context.Adquirientes
                .Include(a => a.Inscripcion)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (adquiriente == null)
            {
                return NotFound();
            }

            return View(adquiriente);
        }

        // GET: Adquirientes/Create
        public IActionResult Create()
        {
            ViewData["InscripcionId"] = new SelectList(_context.Inscripciones, "Folio", "Comuna");
            return View();
        }

        // POST: Adquirientes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,InscripcionId,Rut,PorcentajeDerecho,Acreditado")] Adquiriente adquiriente)
        {
            if (ModelState.IsValid)
            {
                _context.Add(adquiriente);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["InscripcionId"] = new SelectList(_context.Inscripciones, "Folio", "Comuna", adquiriente.InscripcionId);
            return View(adquiriente);
        }

        // GET: Adquirientes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Adquirientes == null)
            {
                return NotFound();
            }

            var adquiriente = await _context.Adquirientes.FindAsync(id);
            if (adquiriente == null)
            {
                return NotFound();
            }
            ViewData["InscripcionId"] = new SelectList(_context.Inscripciones, "Folio", "Comuna", adquiriente.InscripcionId);
            return View(adquiriente);
        }

        // POST: Adquirientes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,InscripcionId,Rut,PorcentajeDerecho,Acreditado")] Adquiriente adquiriente)
        {
            if (id != adquiriente.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(adquiriente);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AdquirienteExists(adquiriente.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["InscripcionId"] = new SelectList(_context.Inscripciones, "Folio", "Comuna", adquiriente.InscripcionId);
            return View(adquiriente);
        }

        // GET: Adquirientes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Adquirientes == null)
            {
                return NotFound();
            }

            var adquiriente = await _context.Adquirientes
                .Include(a => a.Inscripcion)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (adquiriente == null)
            {
                return NotFound();
            }

            return View(adquiriente);
        }

        // POST: Adquirientes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Adquirientes == null)
            {
                return Problem("Entity set 'InscriptionsGrupo5DbContext.Adquirientes'  is null.");
            }
            var adquiriente = await _context.Adquirientes.FindAsync(id);
            if (adquiriente != null)
            {
                _context.Adquirientes.Remove(adquiriente);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AdquirienteExists(int id)
        {
          return (_context.Adquirientes?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
