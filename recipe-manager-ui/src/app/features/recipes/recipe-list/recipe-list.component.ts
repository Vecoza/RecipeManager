import { Component, OnInit, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { debounceTime, distinctUntilChanged, Subject } from 'rxjs';
import { RecipeService } from '../../../core/services/recipe.service';
import { TagService } from '../../../core/services/tag.service';
import { AuthService } from '../../../core/services/auth.service';
import { RecipeListItem, TagDto } from '../../../shared/models/recipe.models';
import { RecipeCardComponent } from '../../../shared/components/recipe-card/recipe-card.component';

@Component({
  selector: 'app-recipe-list',
  standalone: true,
  imports: [RouterLink, FormsModule, MatIconModule, MatButtonModule, MatProgressSpinnerModule, RecipeCardComponent],
  templateUrl: './recipe-list.component.html',
  styleUrl: './recipe-list.component.scss'
})
export class RecipeListComponent implements OnInit {
  recipes = signal<RecipeListItem[]>([]);
  availableTags = signal<TagDto[]>([]);
  selectedTags = signal<string[]>([]);
  loading = signal(false);
  searchTerm = '';

  private searchSubject = new Subject<string>();

  constructor(
    private recipeService: RecipeService,
    private tagService: TagService,
    public auth: AuthService
  ) {}

  ngOnInit() {
    this.loadTags();
    this.loadRecipes();
    this.searchSubject.pipe(debounceTime(300), distinctUntilChanged())
      .subscribe(() => this.loadRecipes());
  }

  loadRecipes() {
    this.loading.set(true);
    const tags = this.selectedTags().join(',') || undefined;
    this.recipeService.getAll(this.searchTerm || undefined, tags).subscribe({
      next: data => { this.recipes.set(data); this.loading.set(false); },
      error: () => this.loading.set(false)
    });
  }

  loadTags() {
    this.tagService.getAll().subscribe(tags => this.availableTags.set(tags));
  }

  onSearch(value: string) { this.searchSubject.next(value); }

  toggleTag(name: string) {
    const current = this.selectedTags();
    this.selectedTags.set(
      current.includes(name) ? current.filter(t => t !== name) : [...current, name]
    );
    this.loadRecipes();
  }

  isTagSelected(name: string) { return this.selectedTags().includes(name); }
}
