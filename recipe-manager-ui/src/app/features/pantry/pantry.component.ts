import { Component, OnInit, signal } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSelectModule } from '@angular/material/select';
import { PantryService } from '../../core/services/pantry.service';
import { RecipeService } from '../../core/services/recipe.service';
import { AuthService } from '../../core/services/auth.service';
import { PantryItem, PantryMatchResult, GeneratedRecipeDto } from '../../shared/models/pantry.models';
import { CreateRecipeDto } from '../../shared/models/recipe.models';

@Component({
  selector: 'app-pantry',
  standalone: true,
  imports: [RouterLink, RouterLinkActive, ReactiveFormsModule, MatIconModule, MatButtonModule, MatFormFieldModule, MatInputModule, MatSelectModule, MatProgressSpinnerModule],
  templateUrl: './pantry.component.html',
  styleUrl: './pantry.component.scss'
})
export class PantryComponent implements OnInit {
  pantryItems = signal<PantryItem[]>([]);
  matchResult = signal<PantryMatchResult | null>(null);
  generatedRecipe = signal<GeneratedRecipeDto | null>(null);

  loadingItems = signal(false);
  loadingMatches = signal(false);
  loadingGenerate = signal(false);
  savingRecipe = signal(false);

  addForm: FormGroup;
  editingId = signal<string | null>(null);
  editForm: FormGroup;

  generateError = signal('');
  savedSuccess = signal(false);

  showPreferences = signal(false);
  preferences = signal({ recipeType: 'Either', mealType: 'Dinner', cookingTime: 'Any' });

  readonly units = ['g', 'kg', 'ml', 'l', 'tsp', 'tbsp', 'cup', 'pcs', 'pinch', 'slice', 'bunch'];

  constructor(
    private pantryService: PantryService,
    private recipeService: RecipeService,
    private fb: FormBuilder,
    public auth: AuthService
  ) {
    this.addForm = this.fb.group({
      name: ['', Validators.required],
      quantity: [1, [Validators.required, Validators.min(0)]],
      unit: ['']
    });
    this.editForm = this.fb.group({
      name: ['', Validators.required],
      quantity: [1, [Validators.required, Validators.min(0)]],
      unit: ['']
    });
  }

  ngOnInit() { this.loadItems(); }

  loadItems() {
    this.loadingItems.set(true);
    this.pantryService.getAll().subscribe({
      next: items => { this.pantryItems.set(items); this.loadingItems.set(false); },
      error: () => this.loadingItems.set(false)
    });
  }

  addItem() {
    if (this.addForm.invalid) return;
    this.pantryService.create(this.addForm.value).subscribe(item => {
      this.pantryItems.update(items => [...items, item]);
      this.addForm.reset({ quantity: 1, unit: '' });
    });
  }

  startEdit(item: PantryItem) {
    this.editingId.set(item.id);
    this.editForm.setValue({ name: item.name, quantity: item.quantity, unit: item.unit });
  }

  saveEdit(id: string) {
    if (this.editForm.invalid) return;
    this.pantryService.update(id, this.editForm.value).subscribe(updated => {
      if (updated) this.pantryItems.update(items => items.map(i => i.id === id ? updated : i));
      this.editingId.set(null);
    });
  }

  cancelEdit() { this.editingId.set(null); }

  deleteItem(id: string) {
    this.pantryService.delete(id).subscribe(() => {
      this.pantryItems.update(items => items.filter(i => i.id !== id));
    });
  }

  findMatches() {
    this.loadingMatches.set(true);
    this.matchResult.set(null);
    this.pantryService.getMatches().subscribe({
      next: result => { this.matchResult.set(result); this.loadingMatches.set(false); },
      error: () => this.loadingMatches.set(false)
    });
  }

  openPreferences() {
    this.showPreferences.set(true);
    this.generatedRecipe.set(null);
    this.generateError.set('');
    this.savedSuccess.set(false);
  }

  cancelPreferences() {
    this.showPreferences.set(false);
  }

  setPreference(key: 'recipeType' | 'mealType' | 'cookingTime', value: string) {
    this.preferences.update(p => ({ ...p, [key]: value }));
  }

  confirmGenerate() {
    this.showPreferences.set(false);
    this.loadingGenerate.set(true);
    this.generateError.set('');
    this.generatedRecipe.set(null);
    this.savedSuccess.set(false);
    this.pantryService.generate(this.preferences()).subscribe({
      next: recipe => { this.generatedRecipe.set(recipe); this.loadingGenerate.set(false); },
      error: () => {
        this.generateError.set("Generation is taking too long or Ollama is not running. Make sure 'ollama serve' is running and try again. First generation can take 2–3 minutes on CPU.");
        this.loadingGenerate.set(false);
      }
    });
  }

  saveGeneratedRecipe() {
    const recipe = this.generatedRecipe();
    if (!recipe) return;
    this.savingRecipe.set(true);

    const dto: CreateRecipeDto = {
      title: recipe.title,
      description: recipe.description,
      prepTimeMinutes: recipe.prepTimeMinutes,
      cookTimeMinutes: recipe.cookTimeMinutes,
      servings: recipe.servings,
      ingredients: recipe.ingredients,
      steps: recipe.steps,
      tagIds: []
    };

    this.recipeService.create(dto).subscribe({
      next: () => { this.savedSuccess.set(true); this.savingRecipe.set(false); },
      error: () => this.savingRecipe.set(false)
    });
  }

  get hasItems() { return this.pantryItems().length > 0; }
}
