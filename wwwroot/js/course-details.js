document.addEventListener('DOMContentLoaded', () => {
  const tabContainer = document.querySelector('.course-tab-nav');
  const contentArea = document.getElementById('tab-content');

  if (!tabContainer || !contentArea) {
    return;
  }

  const setActiveTab = (path) => {
    const anchors = tabContainer.querySelectorAll('.tab-link');
    anchors.forEach((anchor) => {
      anchor.classList.toggle('active', anchor.href === path || anchor.getAttribute('href') === path);
    });
  };

  const buildCommentHtml = (comment) => {
    const wrapper = document.createElement('div');
    wrapper.className = 'd-flex gap-3 mb-3';
    wrapper.innerHTML = `
      <div class="flex-shrink-0">
        <div class="rounded-circle bg-light d-flex align-items-center justify-content-center" style="width: 32px; height: 32px;">
          <span class="small fw-bold text-primary">${comment.CommentorName.charAt(0)}</span>
        </div>
      </div>
      <div class="flex-grow-1 bg-light p-3 rounded-3">
        <div class="d-flex justify-content-between align-items-center mb-1">
          <span class="small fw-bold">${comment.CommentorName}</span>
          <span class="text-muted" style="font-size: 0.75rem;">${comment.CommentedTimeAndDate}</span>
        </div>
        <p class="small mb-0">${comment.Content}</p>
      </div>
    `;
    return wrapper;
  };

  const addCommentToPost = (postId, comment) => {
    const commentsList = contentArea.querySelector(`.comments-list[data-post-id="${postId}"]`);
    const countBadge = contentArea.querySelector(`.comments-count[data-post-id="${postId}"]`);

    if (!commentsList || !countBadge) {
      return;
    }

    const emptyMessage = commentsList.querySelector('.no-comments-message');
    if (emptyMessage) {
      emptyMessage.remove();
    }

    commentsList.appendChild(buildCommentHtml(comment));
    const currentCount = Number(countBadge.textContent.trim()) || 0;
    countBadge.textContent = currentCount + 1;
  };

  const submitComment = async (postId, content, button) => {
    const trimmedContent = content.trim();
    if (!trimmedContent) {
      return;
    }

    button.disabled = true;
    const originalText = button.textContent;
    button.textContent = 'Posting...';

    try {
      const response = await fetch('/Student/AddPostComment', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          'X-Requested-With': 'XMLHttpRequest'
        },
        credentials: 'same-origin',
        body: JSON.stringify({ postId: Number(postId), content: trimmedContent })
      });

      const resultText = await response.text();
      let result;
      try {
        result = JSON.parse(resultText);
      } catch (parseError) {
        if (response.ok) {
          result = {
            success: true,
            comment: {
              PostId: Number(postId),
              CommentorName: 'You',
              Content: trimmedContent,
              CommentedTimeAndDate: new Date().toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })
            }
          };
        } else {
          throw new Error('Unable to parse server response.');
        }
      }

      if (!response.ok || !result?.success) {
        throw new Error(result?.message || 'Unable to post comment');
      }

      addCommentToPost(postId, result.comment);
      const textarea = contentArea.querySelector(`.comment-input[data-post-id="${postId}"]`);
      if (textarea) {
        textarea.value = '';
      }

      try {
        const commentsCollapse = document.getElementById(`commentsCollapse-${postId}`);
        if (commentsCollapse && window.bootstrap?.Collapse) {
          const collapseInstance = bootstrap.Collapse.getOrCreateInstance(commentsCollapse);
          collapseInstance.show();
        }
      } catch (collapseError) {
        console.warn('Could not expand comments panel:', collapseError);
      }
    } catch (error) {
      console.error(error);
      const alert = document.createElement('div');
      alert.className = 'alert alert-danger mt-3';
      alert.textContent = 'Unable to post comment. Please try again.';
      const formGroup = button.closest('.input-group');
      if (formGroup && !formGroup.querySelector('.comment-error')) {
        alert.classList.add('comment-error');
        formGroup.parentElement?.appendChild(alert);
      }
    } finally {
      button.disabled = false;
      button.textContent = originalText;
    }
  };

  const loadTab = async (url, pushState = true) => {
    if (!url) {
      return;
    }

    if (pushState) {
      window.history.pushState(null, '', url);
    }

    contentArea.innerHTML = '<div class="text-center py-5"><div class="spinner-border text-primary me-2" role="status"></div><span class="fw-semibold">Loading...</span></div>';

    try {
      const response = await fetch(url, {
        headers: {
          'X-Requested-With': 'XMLHttpRequest'
        },
        credentials: 'same-origin'
      });

      if (!response.ok) {
        throw new Error('Failed to load content');
      }

      const html = await response.text();
      contentArea.innerHTML = html;
      setActiveTab(window.location.pathname);
    } catch (error) {
      contentArea.innerHTML = '<div class="alert alert-danger">Unable to load the selected tab. Please refresh or try again later.</div>';
      console.error(error);
    }
  };

  tabContainer.addEventListener('click', (event) => {
    const target = event.target.closest('.tab-link');
    if (!target) {
      return;
    }

    const href = target.getAttribute('href');
    if (!href || (href.startsWith('http') && !href.includes(window.location.host))) {
      return;
    }

    event.preventDefault();
    loadTab(href);
  });

  contentArea.addEventListener('click', (event) => {
    const button = event.target.closest('.comment-submit-btn');
    if (!button) {
      return;
    }

    const postId = button.dataset.postId;
    const textarea = contentArea.querySelector(`.comment-input[data-post-id="${postId}"]`);
    if (!postId || !textarea) {
      return;
    }

    submitComment(postId, textarea.value, button);
  });

  window.addEventListener('popstate', () => {
    loadTab(window.location.pathname, false);
  });
});
