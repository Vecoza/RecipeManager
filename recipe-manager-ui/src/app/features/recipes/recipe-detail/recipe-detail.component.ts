import { Component, OnInit, signal, computed } from '@angular/core';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatChipsModule } from '@angular/material/chips';
import { MatDividerModule } from '@angular/material/divider';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatToolbarModule } from '@angular/material/toolbar';
import { RecipeService } from '../../../core/services/recipe.service';
import { AuthService } from '../../../core/services/auth.service';
import { RecipeDetail } from '../../../shared/models/recipe.models';

@Component({
  selector: 'app-recipe-detail',
  standalone: true,
  imports: [
    RouterLink, MatButtonModule, MatIconModule, MatChipsModule,
    MatDividerModule, MatProgressSpinnerModule, MatToolbarModule
  ],
  templateUrl: './recipe-detail.component.html',
  styleUrl: './recipe-detail.component.css'
})
export class RecipeDetailComponent implements OnInit {
  recipe = signal<RecipeDetail | null>(null);
  loading = signal(true);
  scaledServings = signal(0);

  scaledIngredients = computed(() => {
    const r = this.recipe();
    if (!r) return [];
    const ratio = this.scaledServings() / r.servings;
    return r.ingredients.map(ing => ({
      ...ing,
      quantity: Math.round(ing.quantity * ratio * 100) / 100
    }));
  });

  isOwner = computed(() => {
    const r = this.recipe();
    return r ? r.userId === this.getCurrentUserId() : false;
  });

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private recipeService: RecipeService,
    private auth: AuthService
  ) {}

  ngOnInit() {
    const id = this.route.snapshot.paramMap.get('id')!;
    this.recipeService.getById(id).subscribe({
      next: r => {
        this.recipe.set(r);
        this.scaledServings.set(r.servings);
        this.loading.set(false);
      },
      error: () => { this.loading.set(false); this.router.navigate(['/recipes']); }
    });
  }

  adjustServings(delta: number) {
    const next = this.scaledServings() + delta;
    if (next >= 1) this.scaledServings.set(next);
  }

  delete() {
    const id = this.recipe()?.id;
    if (!id) return;
    this.recipeService.delete(id).subscribe(() => this.router.navigate(['/recipes']));
  }

  private getCurrentUserId(): string {
    const token = this.auth.token();
    if (!token) return '';
    try {
      const payload = JSON.parse(atob(token.split('.')[1]));
      return payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'] ?? '';
    } catch { return ''; }
  }
}
