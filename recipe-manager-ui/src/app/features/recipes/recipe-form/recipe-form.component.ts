import { Component, OnInit, signal } from '@angular/core';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { FormArray, FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatSelectModule } from '@angular/material/select';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { RecipeService } from '../../../core/services/recipe.service';
import { TagService } from '../../../core/services/tag.service';
import { TagDto } from '../../../shared/models/recipe.models';

@Component({
  selector: 'app-recipe-form',
  standalone: true,
  imports: [RouterLink, ReactiveFormsModule, MatFormFieldModule, MatInputModule, MatButtonModule, MatIconModule, MatSelectModule, MatProgressSpinnerModule],
  templateUrl: './recipe-form.component.html',
  styleUrl: './recipe-form.component.scss'
})
export class RecipeFormComponent implements OnInit {
  form!: FormGroup;
  availableTags = signal<TagDto[]>([]);
  loading = signal(false);
  saving = signal(false);
  error = signal('');
  isEdit = false;
  private recipeId?: string;

  constructor(
    private fb: FormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    private recipeService: RecipeService,
    private tagService: TagService
  ) {}

  ngOnInit() {
    this.buildForm();
    this.tagService.getAll().subscribe(tags => this.availableTags.set(tags));
    this.recipeId = this.route.snapshot.paramMap.get('id') ?? undefined;
    this.isEdit = !!this.recipeId;

    if (this.isEdit) {
      this.loading.set(true);
      this.recipeService.getById(this.recipeId!).subscribe({
        next: r => {
          this.form.patchValue({
            title: r.title, description: r.description, imageUrl: r.imageUrl,
            prepTimeMinutes: r.prepTimeMinutes, cookTimeMinutes: r.cookTimeMinutes,
            servings: r.servings, tagIds: r.tags.map(t => t.id)
          });
          r.ingredients.forEach(i => this.ingredients.push(this.fb.group({
            name: [i.name, Validators.required], quantity: [i.quantity, Validators.required], unit: [i.unit]
          })));
          r.steps.forEach(s => this.steps.push(this.fb.group({ instruction: [s.instruction, Validators.required] })));
          this.loading.set(false);
        },
        error: () => this.router.navigate(['/recipes'])
      });
    }
  }

  get ingredients() { return this.form.get('ingredients') as FormArray; }
  get steps() { return this.form.get('steps') as FormArray; }

  addIngredient() {
    this.ingredients.push(this.fb.group({ name: ['', Validators.required], quantity: [1, Validators.required], unit: [''] }));
  }

  removeIngredient(i: number) { this.ingredients.removeAt(i); }
  addStep() { this.steps.push(this.fb.group({ instruction: ['', Validators.required] })); }
  removeStep(i: number) { this.steps.removeAt(i); }

  submit() {
    if (this.form.invalid) return;
    this.saving.set(true);
    this.error.set('');
    const dto = this.form.value;
    const request = this.isEdit ? this.recipeService.update(this.recipeId!, dto) : this.recipeService.create(dto);
    request.subscribe({
      next: r => this.router.navigate(['/recipes', r.id]),
      error: err => { this.error.set(err.error?.message ?? 'Save failed'); this.saving.set(false); }
    });
  }

  private buildForm() {
    this.form = this.fb.group({
      title: ['', Validators.required],
      description: [''],
      imageUrl: [''],
      prepTimeMinutes: [0, [Validators.required, Validators.min(0)]],
      cookTimeMinutes: [0, [Validators.required, Validators.min(0)]],
      servings: [4, [Validators.required, Validators.min(1)]],
      tagIds: [[]],
      ingredients: this.fb.array([]),
      steps: this.fb.array([])
    });
  }
}
